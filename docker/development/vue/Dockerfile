FROM node:lts-alpine3.12

RUN apk add --no-cache bash

WORKDIR /app/src

COPY beontime.frontend/package.json .
RUN yarn

# webpack build
COPY beontime.frontend/ .
ENTRYPOINT yarn serve