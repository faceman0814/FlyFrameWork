@echo off
setlocal enabledelayedexpansion

REM 设置项目根目录
set ROOT_DIR=D:\FaceMan\FlyFrameWork

REM 删除bin和obj文件夹
for /d /r "%ROOT_DIR%" %%d in (bin, obj) do (
    if exist "%%d" (
        echo 正在删除: %%d
        rmdir /s /q "%%d"
    )
)

echo 清理完成！
pause