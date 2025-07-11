

## 📌 Gợi ý tối ưu nếu giữ nguyên cấu trúc hiện tại:

1. **Tách docker-compose thành nhiều file con để dễ quản lý**:

```bash
docker-compose.yml              # Gọi chung từ các file bên dưới
docker-compose.backend.yml      # Chứa API, DB, Redis
docker-compose.frontend.yml     # Chứa frontend
docker-compose.keycloak.yml     # Chứa keycloak nếu cần
```

Sau đó gom lại bằng `docker-compose.yml` chính:

```yaml
version: "3.8"
services: {}
networks:
  blognet:

include:
  - docker-compose.backend.yml
  - docker-compose.frontend.yml
  - docker-compose.keycloak.yml
```

> Hoặc nếu không dùng `include`, có thể dùng `-f`:

```bash
docker-compose -f docker-compose.backend.yml -f docker-compose.frontend.yml up
```

2. **Tài liệu rõ ràng trong README.md**: ví dụ

````md
## Run entire stack

```bash
cd Infrastructure
docker-compose up --build
````

```