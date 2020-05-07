# OpenBlog
打造个性化的轻量级独立博客, 暂定官方域名 http://openblog.net

# 前台功能
## 博客基本功能
* 文章列表(默认/分类/tag)
* 查看文章
* 文章支持回复
## 登录认证
* 自有账号登录
* 第三方账号登录(需要可以在后台配置)
### 第三方账号登录支持情况
* 微软
* GITHUB
* QQ
* 微信
* 微博

# 后台功能
## 文章管理
* 文章列表
* 发布文章
### 文章安全
* 文章支持设置密码
* 文章支持带token(有有效期限制)无密码访问
* 支持生成图片+水印形式输出博客，避免复制传播
## 数据管理与安全
* 支持数据的导入导出备份
* 可以轻松迁移，并重建
* 支持直接备份到其他网络位置(网盘/FTP服务器)
## 其他
* 插件, 各个需要插件的场景
* 前台模板支持
* 广告位管理
* 团队博客
* 其他博客文章采集
* 博客以外的其他功能

# 技术栈

## 开发框架
* asp.net core 3.1

## 存储

### 关系型数据存储
* Mongodb
* Postgresql

### Blob数据存储(图片/文档)
* FileSystem
* Mongodb GridFS
## 打包发布
本地通过docker build命令手动打包, 完成打包之后手动上传到公网仓库
## 部署
新版本发布会发布公共仓库的镜像，在安装有docker上可以直接通过docker run运行

# 项目目标
* 可以作为个人的知识库
* 各种新技术的测试

# 参考
* 部分设计会参考[Edi.Wang的博客设计](https://github.com/EdiWang/Moonglade)

# RUN

## OpenBlog Web
```
docker rm -f openblog &&
docker run -d -it --name openblog \
-p 9428:80  \
--restart always  \
--link mongo:mongo \
--volume=/data/openblog/:/appdata \
-e ASPNETCORE_ENVIRONMENT=Staging \
dukecheng/openblog:latest
```

## Mongo
```
mkdir -p /data/mongo/{db,backup,configdb,log}
docker run -d --restart always --name mongo \
--volume=/data/mongo/db:/data/db \
--volume=/data/mongo/backup:/data/backup \
--volume=/data/mongo/configdb:/data/configdb  \
--volume=/data/mongo/log:/var/log  \
hub.niusys.com/library/mongo:latest
```

## 国内镜像地址
```
docker pull hub.niusys.com/dukecheng/openblog:latest
docker pull hub.niusys.com/library/mongo:latest
```

## 安装(查看Install Token)
```
docker logs openblog
看到下面类似的token
==============================================================
Init Token
BM0XvkdXNIB7XpJ5OVWNtL1ku5GFl07wyI5aVA6TVKg=
==============================================================
```

## Blazor APP Publish
```
dotnet publish --output OpenBlog.Web/wwwroot/blazorapp/UserCenterWebApp OpenBlog.UserCenterWeb/OpenBlog.UserCenterWeb.csproj
dotnet publish --output OpenBlog.Web/wwwroot/blazorapp/AdminWebApp OpenBlog.AdminWeb/OpenBlog.AdminWeb.csproj
```

## 项目说明

### OpenBlog.Web
服务器端(前端页面/后端页面/API等)

### OpenBlog.UserCenterWeb
用户Profile对应的前端

### OpenBlog.AdminWeb
管理后台