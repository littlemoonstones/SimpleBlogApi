# About Simple Blog API
This is a straightforward blog API.

## Features
- Authentication and authorization using JWT tokens
- API endpoints for registering and logging in users
- API endpoints for retrieving, creating, updating and deleting posts
- API endpoints for retrieving, creating and deleting comments
- Clean architecture
- Unit tests for the application

## Projects
This is a solution for a blog application, structured into several projects:
- [Blog.Core](Blog.Core/): Contains the core business logic, DTOs, entities, interfaces, and services.
- [Blog.Infrastructure](Blog.Infrastructure/): Contains the infrastructure logic, including the database context and repositories.
- [Blog.UnitTests](Blog.UnitTests/): Contains unit tests for the application.
- [Blog.Api](Blog.Api/): The API project for the application.

# Installation
To set up the project locally, follow these steps:

1. Clone the repository: 
```bash
git clone https://github.com/littlemoonstones/SimpleBlogApi.git
```

2. Navigate to the project directory:
```bash
cd SimpleBlogApi
```

3. Add your JWT Key
```bash
dotnet user-secrets set "Jwt:Key" "your-key"
```

4. Restore the .NET packages:
```bash
dotnet restore
```

5. Build the project:
```bash
dotnet build
```

6. Run the application
```bash
dotnet run --project Blog.Api
```

## Running the Tests

```bash
dotnet test
```

## API Endpoints
The Blog Web API has the following endpoints:

### `POST /api/auth/register`

This endpoint is used to register a new user.

#### Request Body

- `UserName`: string (required)
- `Email`: string (required)
- `Password`: string (required)

#### Responses

- `200 OK`: User registration was successful. Returns a message indicating the user was created successfully.
- `400 Bad Request`: User registration failed. This could be due to invalid input or the password not meeting the requirements. Returns a list of error descriptions.

---

### `POST /api/auth/login`

This endpoint is used to log in a user.

#### Request Body

- `Email`: string (required)
- `Password`: string (required)

#### Responses

- `200 OK`: User login was successful. Returns a JWT token.
- `400 Bad Request`: User login failed. This could be due to invalid input or incorrect email/password. Returns an error message.

---

### `POST /api/auth/logout`

This endpoint is used to log out a user. Requires user to be authenticated.

#### Responses

- `200 OK`: User logout was successful.
- `401 Unauthorized`: User is not authenticated.

### `POST /api/post`

Creates a new post.

#### Request Body

- `PostRequestDto` object:
    - `Title`: string (required)
    - `Content`: string (required)

#### Response

- `200 OK`: Returns the created post as a `PostResponseDto` object.
- `400 Bad Request`: If the model state is not valid or an exception occurs.
- `403 Forbidden`: If an unauthorized access exception occurs.

### `GET /api/post/{id}`

Fetches a post by its ID.

#### Path Parameters

- `id`: GUID of the post.

#### Response

- `200 OK`: Returns the requested post as a `PostResponseDto` object.
- `400 Bad Request`: If an exception occurs.
- `404 Not Found`: If the post or its author does not exist.

### `DELETE /api/post/{id}`

Deletes a post by its ID.

#### Path Parameters

- `id`: GUID of the post.

#### Response

- `204 No Content`: If the post is successfully deleted.
- `400 Bad Request`: If an exception occurs.
- `403 Forbidden`: If an unauthorized access exception occurs.
- `404 Not Found`: If the post does not exist.

### `PUT /api/post/{id}`

Updates a post by its ID.

#### Path Parameters

- `id`: GUID of the post.

#### Request Body

- `PostRequestDto` object:
    - `Title`: string (required)
    - `Content`: string (required)

#### Response

- `200 OK`: Returns the updated post as a `PostResponseDto` object.
- `400 Bad Request`: If the model state is not valid or an exception occurs.
- `403 Forbidden`: If an unauthorized access exception occurs.
- `404 Not Found`: If the post does not exist.

Please note that the `POST`, `DELETE`, and `PUT` endpoints require authorization.

### `GET /api/post/{postId}/comments`

Fetches comments by the post ID.

#### Path Parameters

- `postId`: GUID of the post.

#### Response

- `200 OK`: Returns the requested comments as a list of `CommentResponseDto` objects.
- `400 Bad Request`: If an exception occurs.

### `POST /api/post/{postId}/comment`

Creates a new comment for a post.

#### Path Parameters

- `postId`: GUID of the post.

#### Request Body

- `CommentRequestDto` object:
    - `Content`: string (required)

#### Response

- `200 OK`: Returns the created comment as a `CommentResponseDto` object.
- `400 Bad Request`: If the model state is not valid or an exception occurs.
- `403 Forbidden`: If an unauthorized access exception occurs.
- `404 Not Found`: If the post does not exist.

### `DELETE /api/post/{postId}/comment/{commentId}`

Deletes a comment by its ID.

#### Path Parameters

- `postId`: GUID of the post.
- `commentId`: GUID of the comment.

#### Response

- `204 No Content`: If the comment is successfully deleted.
- `400 Bad Request`: If an exception occurs.
- `403 Forbidden`: If an unauthorized access exception occurs.
- `404 Not Found`: If the comment does not exist.

Please note that the `POST` and `DELETE` endpoints require authorization.

## License
This project is licensed under the MIT License.
Feel free to customize it further based on your specific project details and preferences.
