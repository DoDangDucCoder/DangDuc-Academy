export class DecodeDataReturn {
  decodeDataReturn<T>(dataReturn: string): T | null {
    if (!dataReturn) {
      return null;
    }

    try {
      const decodedBinary = window.atob(dataReturn);
      const decodedBytes = Uint8Array.from(decodedBinary, (value) => value.charCodeAt(0));
      const dataDecode = new TextDecoder().decode(decodedBytes);
      return JSON.parse(dataDecode) as T;
    } catch {
      return null;
    }
  }
}
