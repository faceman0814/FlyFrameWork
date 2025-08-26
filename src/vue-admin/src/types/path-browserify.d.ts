declare module 'path-browserify' {
  interface PlatformPath {
    normalize(p: string): string;
    join(...paths: string[]): string;
    resolve(...pathSegments: string[]): string;
    isAbsolute(p: string): boolean;
    relative(from: string, to: string): string;
    dirname(p: string): string;
    basename(p: string, ext?: string): string;
    extname(p: string): string;
    parse(p: string): ParsedPath;
    format(pP: FormatInputPathObject): string;
    readonly sep: string;
    readonly delimiter: string;
    readonly posix: PlatformPath;
    readonly win32: PlatformPath;
  }

  interface ParsedPath {
    root: string;
    dir: string;
    base: string;
    ext: string;
    name: string;
  }

  interface FormatInputPathObject {
    root?: string;
    dir?: string;
    base?: string;
    name?: string;
    ext?: string;
  }

  const path: PlatformPath;
  export = path;
}