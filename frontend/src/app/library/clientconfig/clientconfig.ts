import clientConfig from '../../../assets/document/client-config.json';

export class ClientConfig {
  private static resClientConfig: any = clientConfig;
  private static charSplit = ';';
  private static keyClientConfig = 'ClientConfig';

  public static GetResClientConfig(pKey: string) {
    return this.resClientConfig?.[this.keyClientConfig]?.[pKey];
  }

  public static GetPageSizeOptions(): Array<string> {
    const value = this.resClientConfig?.[this.keyClientConfig]?.PageSizeOptions ?? '';
    return value.split(this.charSplit).filter((item: string) => item.length > 0);
  }

  public static GetPageLabel(): string {
    return this.resClientConfig?.[this.keyClientConfig]?.PageLabel ?? '';
  }

  public static GetFunction(): string {
    return this.resClientConfig?.[this.keyClientConfig]?.Function ?? '';
  }
}

