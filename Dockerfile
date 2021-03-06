FROM microsoft/dotnet:1.1.1-sdk

RUN apt-get update
RUN apt-get install -y build-essential nodejs nodejs-legacy npm
RUN curl -sL https://deb.nodesource.com/setup_6.x | bash -
RUN apt-get install -y nodejs

WORKDIR /app

#COPY ontap.csproj .
COPY . /app
RUN ["dotnet", "restore"]
RUN ["dotnet", "build"]


#RUN ["node", "-v"]
#RUN ["npm", "-v"]
#RUN ["npm", "install"]
#RUN ["npm", "run", "webpack-cfg"]
#RUN ["npm", "run", "webpack"]

ENV ASPNETCORE_URLS=http://0.0.0.0:5000
EXPOSE 5000

ENTRYPOINT ["dotnet", "run", "--server.urls", "http://0.0.0.0:5000"]
