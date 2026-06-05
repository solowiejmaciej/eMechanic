FROM node:22-alpine
WORKDIR /app
COPY package.json package-lock.json ./
RUN --mount=type=cache,target=/root/.npm \
    npm ci --fetch-retries=5 --fetch-retry-mintimeout=10000 --fetch-retry-maxtimeout=60000
COPY . .

ENV VITE_PORT=4173

EXPOSE 4173
CMD ["npm", "run", "dev", "--", "--host", "0.0.0.0"]
