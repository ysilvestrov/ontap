FROM microsoft/dotnet:1.1.0-sdk-projectjson

RUN apt-get update
RUN apt-get install -y build-essential nodejs nodejs-legacy npm
RUN curl -sL https://deb.nodesource.com/setup_6.x | bash -
RUN apt-get install -y nodejs

WORKDIR /app

COPY project.json .
RUN ["dotnet", "restore"]

COPY . /app
RUN ["dotnet", "build"]

RUN ["node", "-v"]
RUN ["npm", "-v"]
RUN ["npm", "install"]
RUN ["npm", "install"]
RUN ["node_modules/.bin/webpack", "--config webpack.config.vendor.js", "--env.prod"]
RUN ["node_modules/.bin/webpack", "--env.prod"]

EXPOSE 80/tcp

ENTRYPOINT ["dotnet", "run", "--server.urls", "http://0.0.0.0:80"]
