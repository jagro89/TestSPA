# BgkTestspaClient

This project was generated using [Angular CLI](https://github.com/angular/angular-cli) version 22.2.2.

## Development server

To start a local development server, run:

```bash
ng serve
```

Once the server is running, open your browser and navigate to `http://localhost:4200/`. The application will automatically reload whenever you modify any of the source files.

## Code scaffolding

Angular CLI includes powerful code scaffolding tools. To generate a new component, run:

```bash
ng generate component component-name
```

For a complete list of available schematics (such as `components`, `directives`, or `pipes`), run:

```bash
ng generate --help
```

## Building

Set `BASE_HREF` to the URL path where the application will be hosted, then run the build:

```bash
BASE_HREF=/test/ npm run build
```

On PowerShell:

```bash
$env:BASE_HREF = "/test/"
npm run build
```

You can also pass Angular CLI arguments through the build script, for example `npm run build -- --base-href=/moja-sciezka/`. An explicit `--base-href` argument takes precedence over `BASE_HREF`.

Configure the ASP.NET Core application with the same path using `PathBase` (for example, the Kubernetes environment variable `PathBase=/test`). Configure the ingress to forward requests with the prefix intact so ASP.NET Core can apply the path base. Set `Keycloak:RedirectUri` to the public origin ending in `/signin-oidc` (for example, `https://mojaaplikacja.pl/signin-oidc`); the server adds the path base to the OIDC callback. Register `https://mojaaplikacja.pl/test/signin-oidc` as an allowed redirect URI in Keycloak.

The default base URL is `/`, so omit `BASE_HREF` when hosting at the domain root and leave `PathBase` unset.

The build artifacts are stored in the `dist/` directory. By default, the production build optimizes your application for performance and speed.

## Running unit tests

To execute unit tests with the [Vitest](https://vitest.dev/) test runner, use the following command:

```bash
ng test
```

## Running end-to-end tests

For end-to-end (e2e) testing, run:

```bash
ng e2e
```

Angular CLI does not come with an end-to-end testing framework by default. You can choose one that suits your needs.

## Additional Resources

For more information on using the Angular CLI, including detailed command references, visit the [Angular CLI Overview and Command Reference](https://angular.dev/tools/cli) page.
