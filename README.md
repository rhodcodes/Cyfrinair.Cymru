# Cyfrinair.Cymru

A project for generating secure passwords and Welsh language passphrases. The website is available at `https://cyfrinair.cymru`

## API

The API is available at `https://api.cyfrinair.cymru`

### API Endpoints

#### Generate Password

`GET /password/{count?}`

Generates one or more random passwords.

**Parameters:**

- `count` (path, optional): Number of passwords to generate (1-65535, default: 1)
- `numbers` (query, optional): Include numbers (default: true)
- `symbols` (query, optional): Include special characters (default: true)
- `ambiguous` (query, optional): Include ambiguous characters like 1/l/I (default: true)

**Example Request:**

```http
GET /password/2?numbers=true&symbols=false&ambiguous=false
```

**Example Response**

```json
["5Y6rpeLjjQEnJASDt4PMeD2an", "AmprfaKUvv8qZFz53TruUjjap"]
```

#### Generate Passphrase

`GET /passphrase/{count?}`

Generates one or more Welsh language passphrases.

**Parameters**

- `count` (path, optional): Number of passphrases to generate (1-65535, default: 1)
- `words` (query, optional): Number of words per passphrase (default: 4)
- `separator` (query, optional): Character to separate words (default: -)
- `casing` (query, optional): Word casing (upper/lower/random, default: upper)
- `digit` (query, optional): Digit placement (once/none/every, default: once)

**Example Request**

```http
GET /passphrase/2/?words=5&casing=random
```

**Example Response**

```json
[
  "Curnen-ffugdraed0-ymdoddi-ffinia-Symiant",
  "Cywelyaid-amrywiaeth-dihysbydd0-sybachi-ymgofrestr"
]
```

#### Error Responses

Endpoints return 400 Bad Request for invalid route parameters.

```json
{
  "error": "InvalidCount",
  "message": "You must generate at least one password."
}
```

#### Security

The API implements security best practises including

- HTTPS Only
- Security Headers (HSTS, CSP, X-Frame-Options)
- No caching
- NO CORS (same-origin only)
