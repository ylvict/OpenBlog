﻿FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
ARG source
WORKDIR /app
EXPOSE 80
COPY ${source:-OpenBlog.Web/obj/Docker/publish} .
VOLUME /appdata
ENTRYPOINT ["dotnet", "OpenBlog.Web.dll"]