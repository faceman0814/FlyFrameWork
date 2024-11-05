<script setup lang="ts">
import { hasAuth, getAuths } from "@/router/utils";

defineOptions({
  name: "PermissionButtonRouter"
});
</script>

<template>
  <div>
    <p class="mb-2">当前拥有的code列表：{{ getAuths() }}</p>

    <el-card shadow="never" class="mb-2">
      <template #header>
        <div class="card-header">组件方式判断权限</div>
      </template>
      <el-space wrap>
        <Auth value="User.Node.Create">
          <el-button plain type="warning">
            拥有code：'User.Node.Create' 权限可见
          </el-button>
        </Auth>
        <Auth :value="['User.Node.Update']">
          <el-button plain type="primary">
            拥有code：['User.Node.Update'] 权限可见
          </el-button>
        </Auth>
        <Auth
          :value="['User.Node.Create', 'User.Node.Update', 'User.Node.Delete']"
        >
          <el-button plain type="danger">
            拥有code：['User.Node.Create', 'User.Node.Update',
            'User.Node.Delete'] 权限可见
          </el-button>
        </Auth>
      </el-space>
    </el-card>

    <el-card shadow="never" class="mb-2">
      <template #header>
        <div class="card-header">函数方式判断权限</div>
      </template>
      <el-space wrap>
        <el-button v-if="hasAuth('User.Node.Create')" plain type="warning">
          拥有code：'User.Node.Create' 权限可见
        </el-button>
        <el-button v-if="hasAuth(['User.Node.Update'])" plain type="primary">
          拥有code：['User.Node.Update'] 权限可见
        </el-button>
        <el-button
          v-if="
            hasAuth([
              'User.Node.Create',
              'User.Node.Update',
              'User.Node.Delete'
            ])
          "
          plain
          type="danger"
        >
          拥有code：['User.Node.Create', 'User.Node.Update', 'User.Node.Delete']
          权限可见
        </el-button>
      </el-space>
    </el-card>

    <el-card shadow="never">
      <template #header>
        <div class="card-header">
          指令方式判断权限（该方式不能动态修改权限）
        </div>
      </template>
      <el-space wrap>
        <el-button v-auth="'User.Node.Create'" plain type="warning">
          拥有code：'User.Node.Create' 权限可见
        </el-button>
        <el-button v-auth="['User.Node.Update']" plain type="primary">
          拥有code：['User.Node.Update'] 权限可见
        </el-button>
        <el-button
          v-auth="['User.Node.Create', 'User.Node.Update', 'User.Node.Delete']"
          plain
          type="danger"
        >
          拥有code：['User.Node.Create', 'User.Node.Update', 'User.Node.Delete']
          权限可见
        </el-button>
      </el-space>
    </el-card>
  </div>
</template>
