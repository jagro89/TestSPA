import { spawnSync } from 'node:child_process';
import { resolve } from 'node:path';

const args = [resolve('node_modules/@angular/cli/bin/ng.js'), 'build', ...process.argv.slice(2)];
const hasBaseHrefArgument = args.some((argument) => argument === '--base-href' || argument.startsWith('--base-href='));

if (process.env.BASE_HREF && !hasBaseHrefArgument) {
  args.push('--base-href', process.env.BASE_HREF);
}

const result = spawnSync(process.execPath, args, { stdio: 'inherit' });

if (result.error) {
  throw result.error;
}

process.exit(result.status ?? 1);
