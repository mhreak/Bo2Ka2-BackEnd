# Development seed users

Sample accounts and shop data are created only in Development and Staging. All accounts below are active. Password authentication is available for every role and does not require OTP.

| Role | Username | Mobile | Password | Login endpoint | Request body |
|---|---|---|---|---|---|
| admin | `admin_demo` | `09100000001` | `Seed@12345` | `POST /api/v1/admin/auth/login` | `{"username":"admin_demo","password":"Seed@12345"}` |
| customer | `customer_demo_1` | `09100001001` | `Seed@12345` | `POST /api/v1/customer/auth/login-by-password` | `{"username":"customer_demo_1","password":"Seed@12345"}` |
| customer | `customer_demo_2` | `09100001002` | `Seed@12345` | `POST /api/v1/customer/auth/login-by-password` | `{"username":"customer_demo_2","password":"Seed@12345"}` |
| customer | `customer_demo_3` | `09100001003` | `Seed@12345` | `POST /api/v1/customer/auth/login-by-password` | `{"username":"customer_demo_3","password":"Seed@12345"}` |
| customer | `customer_demo_4` | `09100001004` | `Seed@12345` | `POST /api/v1/customer/auth/login-by-password` | `{"username":"customer_demo_4","password":"Seed@12345"}` |
| customer | `customer_demo_5` | `09100001005` | `Seed@12345` | `POST /api/v1/customer/auth/login-by-password` | `{"username":"customer_demo_5","password":"Seed@12345"}` |
| user_organization | `organization_demo_1` | `09100002001` | `Seed@12345` | `POST /api/v1/user-organization/auth/login-by-password` | `{"username":"organization_demo_1","password":"Seed@12345"}` |
| user_organization | `organization_demo_2` | `09100002002` | `Seed@12345` | `POST /api/v1/user-organization/auth/login-by-password` | `{"username":"organization_demo_2","password":"Seed@12345"}` |
| shop | `shop_demo_1` | `09120000001` | `ShopDemo123` | `POST /api/v1/shop/auth/login-by-password` | `{"username":"shop_demo_1","password":"ShopDemo123"}` |
| shop | `shop_demo_2` | `09120000002` | `ShopDemo123` | `POST /api/v1/shop/auth/login-by-password` | `{"username":"shop_demo_2","password":"ShopDemo123"}` |
| shop | `shop_demo_3` | `09120000003` | `ShopDemo123` | `POST /api/v1/shop/auth/login-by-password` | `{"username":"shop_demo_3","password":"ShopDemo123"}` |
| shop | `shop_demo_4` | `09120000004` | `ShopDemo123` | `POST /api/v1/shop/auth/login-by-password` | `{"username":"shop_demo_4","password":"ShopDemo123"}` |
| shop | `shop_demo_5` | `09120000005` | `ShopDemo123` | `POST /api/v1/shop/auth/login-by-password` | `{"username":"shop_demo_5","password":"ShopDemo123"}` |
| shop | `shop_demo_6` | `09120000006` | `ShopDemo123` | `POST /api/v1/shop/auth/login-by-password` | `{"username":"shop_demo_6","password":"ShopDemo123"}` |

Example:

```bash
curl -X POST http://localhost:5000/api/v1/customer/auth/login-by-password \
  -H "Content-Type: application/json" \
  -d '{"username":"customer_demo_1","password":"Seed@12345"}'
```

For OTP login, first call the same role's `POST /api/v1/{role}/auth/send-otp-for-login` with `{"phoneNumber":"<mobile>"}`, then call `POST /api/v1/{role}/auth/login-otp` with `{"phoneNumber":"<mobile>","code":"<code>"}`. Development uses `RedisOtpService`; there is no fixed OTP or mock sender. `Otp:ExposeCodeInResponse` is not enabled in the checked-in Development settings, so the generated code is not returned by the API and delivery depends on the configured SMS sender. Use password login for the listed accounts unless OTP delivery and code exposure have been configured locally.