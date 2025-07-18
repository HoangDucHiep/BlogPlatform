# Debug JWT Token

## Cách kiểm tra JWT Token:

1. **Decode JWT Token**: Sử dụng https://jwt.io để decode access token và xem các claims
2. **Kiểm tra các claims quan trọng**:
   - `sub` (subject) - ID của user trong Keycloak
   - `azp` (authorized party) - phải match với "lumoblog-auth-client" trong config
   - `iss` (issuer) - phải match với "http://idprovider:8080/realms/LumoBlog"
   - `exp` (expiration) - token chưa hết hạn
   - `iat` (issued at) - thời gian tạo token

## Các bước debug:

1. Login và lấy access token
2. Decode token tại jwt.io
3. Kiểm tra các claims có đúng không
4. Kiểm tra thời gian hết hạn
5. Kiểm tra audience và issuer

## Cấu hình hiện tại:
- Audience: "lumoblog-auth-client"
- ValidIssuer: "http://idprovider:8080/realms/LumoBlog"
- MetadataUrl: "http://idprovider:8080/realms/LumoBlog/.well-known/openid-configuration"

## Các vấn đề thường gặp:

1. **Sai Audience**: Token có `azp` là "lumoblog-auth-client", nhưng cấu hình có `Audience` là "account"
2. **Sai Issuer**: Token có `iss` là "http://localhost:8080/realms/LumoBlog", nhưng cấu hình có `ValidIssuer` là "http://idprovider:8080/realms/LumoBlog"
3. **Thiếu Claims**: Token không có claim `ClaimTypes.NameIdentifier`, chỉ có claim `sub`
4. **Token hết hạn**: Token đã hết hạn (kiểm tra `exp` claim)
5. **Sai định dạng Authorization header**: Header phải là `Authorization: Bearer <token>`

## Các thay đổi đã thực hiện:

1. Cập nhật `Audience` trong cấu hình thành "lumoblog-auth-client"
2. Cập nhật `ClaimsPrincipalExtensions.GetIdentityId()` để sử dụng claim `sub` nếu không tìm thấy claim `NameIdentifier`
3. Cập nhật `JwtBearerOptionsSetup` để cấu hình `TokenValidationParameters` chi tiết hơn
4. Thêm logging để debug quá trình xác thực

## Kiểm tra database:

Đảm bảo có user trong database với `identity_id` khớp với `sub` claim trong token:
```sql
SELECT * FROM users WHERE identity_id = 'f3fdc1c5-e525-4307-b768-73310673dac3';
```