---
applyTo: "**/*.cs"
---
# FlyFramework AI Coding 快速指令（精炼版）

> 面向 AI 代理 / Copilot：下面 40 行覆盖在本仓库高效交付功能所需的最小必知集；深入细节参见后续“详细附录”。

## 1. 架构速览

Mono Repo：`vue-admin/` 前端 (Vite+Vue3)、`aspnet-core/` 自写架构后端 用NSwag 生成客户端服务层。后端模块依赖链固定：Core→Application→EFCore→WebCore→WebHost；禁止跨层直接绕过 Application/Repository。

## 2. 关键生成与禁止事项

所有 HTTP 调用必须使用自动生成 `vue-admin/src/api/service-proxies.ts`禁止手写 `fetch/axios` 指向后台业务接口。生成流程：后端 Controller 变更 → `cd vue/nswag && ./refresh.bat`（或根 nswag 目录）→ 提交更新后的代理文件。

## 3. 前端领域与混入模式

业务代码按域放 `vue/src/app/{knight|admin|account|esign|main}`，跨域复用放 `vue/src/shared/`。所有业务组件 `mixins: [AppComponentBase]`；使用动态表格页面必须再混入 `DynamicTableComponentBase` 并提供 `fetchDataList` 返回 `{ items, totalCount }`。

## 4. 动态表格（核心高频）

后端配置三件套目录：`aspnet-core/src/YoyoBoot.Template.Web.Host/wwwroot/ConfigFiles/{ButtonGroups|PageFilters|ListViews}`；同名 JSON 组成一个表格类型。前端组件字段：`tableType/toolbarType/filterType` 与文件名对应。常用方法：`this.reload()`、`this.getSearchProps()`、`this.getSelectRowKeys()`。典型实现：

```ts
async fetchDataList(p){const input={...p,...this.getSearchProps()};const r=await this.userSvc.getPagedList(input);return{items:r.items||[],totalCount:r.totalCount||0};}
```

## 5. 路由与菜单合并

最终路由 = 后端 `Menu.json` (动态) + 前端静态；合并逻辑在 `vue/src/shared/store/vbenModules/permission.ts`，冲突检测 `vue/src/router/guard/permissionGuard.ts`。新增页面前先确认后端是否已有同名，避免重复路由名（控制台🔥警告）。

## 6. 权限/按钮控制

代码中权限：`this.permission.isGranted('Permission.Name')`；模板按钮显示依赖动态表格 JSON `acl` 字段；新增按钮只改 JSON，不在组件硬编码权限逻辑。

## 7. 性能与构建工作流

安装：`pnpm install --frozen-lockfile`；开发：`pnpm dev:monitor` (优先) 或 `pnpm dev`；快速构建：`pnpm build:fast`；排查怪异缓存：`pnpm clean:cache`；后端运行：`dotnet run --project aspnet-core/src/YoyoBoot.Template.Web.Host`；数据库迁移：`dotnet ef database update`。

## 8. 提交与约定

提交使用 `pnpm commit`（conventional commits）。禁止把大规模格式化和业务改动混在一个 commit。路径别名：`/@/ -> src`，`/#/ -> types`，`@/ -> public`；禁止深层相对路径穿越 (如 `../../../`).

## 9. 常见陷阱（立即规避）

1) 忘记混入 `DynamicTableComponentBase` 导致表格方法缺失。2) `fetchDataList` 未返回 `{ items,totalCount }`。3) 新接口改动后未同步刷新 NSwag 代理。4) 手写 API 调用绕过代理。5) 重复命名路由或 JSON 三件套文件不一致。

## 10. 最小列表页示例

```ts
export default {name:'UserList',mixins:[AppComponentBase,DynamicTableComponentBase],data(){return{tableType:'user',userSvc:new UserServiceProxy()};},async fetchDataList(p){const r=await this.userSvc.getPagedList({...p,...this.getSearchProps()});return{items:r.items||[],totalCount:r.totalCount||0};}};
```

## 11. 原则速记

配置驱动 > 硬编码；生成代码 > 手写重复；保持后端菜单/表格配置为真源；先查 `ConfigFiles` 再新增；跨端统一 DTO。

---
