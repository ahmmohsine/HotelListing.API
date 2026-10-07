# JWT Authentication System Verification Guide

## ✅ Issues Fixed

### 1. HTTP0012 Error - Root Cause
**Problem**: The `.http` file had multiple issues:
- ✗ Duplicate host definitions (line 1: `https://localhost:7166` and line 80: `https://localhost:7082`)
- ✗ Inconsistent ports causing routing failures
- ✗ Missing JWT Bearer tokens on protected endpoints

**Solution**: Consolidated to single port (`localhost:7082`) with proper JWT integration.

### 2. Missing OpenAPI Endpoint Mapping
**Problem**: `AddOpenApi()` was registered but not mapped to an endpoint.
- ✗ `ServiceCollectionExtensions.cs` called `services.AddOpenApi()` 
- ✗ But no `app.MapOpenApi()` in Program.cs or middleware

**Solution**: Added `app.MapOpenApi()` in Program.cs and ApplicationBuilderExtensions.

---

## 🔐 JWT Authentication Flow

### Step 1: Login to Get Token
```http
POST https://localhost:7082/api/auth/login
Content-Type: application/json

{
  "email": "admin@hotellisting.com",
  "password": "AdminPassword123!"
}
```

**Response** (example):
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "email": "admin@hotellisting.com",
  "userId": "user-id-here"
}
```

### Step 2: Extract and Use Token
The `.http` file automatically extracts the token:
```http
@authToken = {{login.response.body.token}}
```

### Step 3: Send Token with Protected Requests
All protected endpoints require the Bearer token:
```http
GET https://localhost:7082/api/hotels
Authorization: Bearer {{authToken}}
```

---

## 🔍 JWT Configuration Verification

### appsettings.json
```json
"JwtSettings": {
  "Secret": "VOTRUQBNOrnQlzo3ftqm0Jj5Sf9zEHlPApapd-rWsAHREzkweiTw",
  "Issuer": "https://api.votre-domaine.com",
  "Audience": "https://votre-app-angular.com",
  "ExpirationInMinutes": 60
}
```

### Program.cs Authentication Order (CRITICAL)
```csharp
app.UseAuthentication();  // 1. Validates JWT token
app.UseAuthorization();   // 2. Checks [Authorize] attributes
```

⚠️ **Order is critical**: Authentication MUST come before Authorization!

### ServiceCollectionExtensions.cs Flow
1. ✅ `AddIdentityCore<ApplicationUser>()` - Sets up user management
2. ✅ `.AddRoles<IdentityRole>()` - Enables role-based authorization
3. ✅ `.AddEntityFrameworkStores<HotelListingDbContext>()` - DB persistence
4. ✅ `AddAuthentication()` - JWT Bearer scheme
5. ✅ `AddJwtBearer()` - Token validation parameters
6. ✅ `AddAuthorization()` - Authorization policies

---

## 🧪 Testing JWT Authentication

### Using the .http file (Recommended)

1. **First**, execute the login request to get a token:
   - Look for the section: `### 1. AUTHENTICATION - CONNEXION`
   - Send the login request
   - Token is automatically extracted

2. **Then**, execute any protected endpoint:
   - All endpoints under "3. HOTELS" or "4. COUNTRIES" sections
   - Token is automatically added to `Authorization: Bearer` header

### Expected Success Response
```
HTTP/1.1 200 OK
Content-Type: application/json

[
  { "id": 1, "name": "Hotel A", ... },
  { "id": 2, "name": "Hotel B", ... }
]
```

### Expected Error Responses

**Missing Token** (401 Unauthorized):
```
HTTP/1.1 401 Unauthorized
```

**Expired Token** (401 Unauthorized):
```
HTTP/1.1 401 Unauthorized
www-authenticate: Bearer error="invalid_token", error_description="token has expired"
```

**Invalid Signature** (401 Unauthorized):
```
HTTP/1.1 401 Unauthorized
```

---

## 📋 Endpoints Reference

### Authentication (Public)
```
POST   /api/auth/login       - Login and get JWT token
POST   /api/auth/register    - Register new user
```

### Hotels (Protected - Requires JWT)
```
GET    /api/hotels           - Get all hotels
GET    /api/hotels/{id}      - Get hotel by ID
POST   /api/hotels           - Create new hotel
PUT    /api/hotels/{id}      - Update hotel
DELETE /api/hotels/{id}      - Delete hotel
```

### Countries (Protected - Requires JWT)
```
GET    /api/countries        - Get all countries
GET    /api/countries/{id}   - Get country by ID
POST   /api/countries        - Create new country
PUT    /api/countries/{id}   - Update country
DELETE /api/countries/{id}   - Delete country
```

---

## 🔧 Common HTTP0012 Error Scenarios

### Scenario 1: Wrong Port
- ✗ Request: `https://localhost:7166/api/hotels`
- ✓ Correct: `https://localhost:7082/api/hotels`

**Fix**: Use consistent port (7082)

### Scenario 2: Missing Bearer Token on Protected Endpoint
- ✗ Request without token:
  ```http
  GET https://localhost:7082/api/hotels
  ```
- ✓ Correct with token:
  ```http
  GET https://localhost:7082/api/hotels
  Authorization: Bearer {token}
  ```

**Fix**: Use the `.http` file format with `@authToken` variable

### Scenario 3: Malformed Authorization Header
- ✗ `Authorization: {authToken}` (missing "Bearer")
- ✓ `Authorization: Bearer {authToken}`

**Fix**: Always use "Bearer" prefix

### Scenario 4: HTTPS Certificate Issues
If you see certificate warnings:
```
Endpoint certificate validation is disabled!
```

This is normal for `localhost` development.

---

## ✅ Verification Checklist

- [ ] Port is consistent: `localhost:7082` throughout `.http` file
- [ ] Login endpoint executed first to get token
- [ ] Token extracted: `@authToken = {{login.response.body.token}}`
- [ ] All protected endpoints use `Authorization: Bearer {{authToken}}`
- [ ] Program.cs includes both:
  - `app.UseAuthentication();`
  - `app.UseAuthorization();`
  - `app.MapOpenApi();` (for .NET 10 native OpenAPI)
- [ ] No duplicate host definitions in `.http` file
- [ ] `appsettings.json` has valid JWT settings
- [ ] Build completes without errors

---

## 🚀 Next Steps

1. **Run the application** - Press F5 in Visual Studio
2. **Open the .http file** - `HotelListing.API.http`
3. **Execute login first** - This gets your JWT token
4. **Test protected endpoints** - They'll automatically use the token

All HTTP0012 errors should now be resolved! 🎉
