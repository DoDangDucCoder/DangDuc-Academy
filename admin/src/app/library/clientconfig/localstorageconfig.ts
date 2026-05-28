export class LocalStorageConfig {

  private static currentUserKey = 'currentUser';

  public static RemoveUser() {
    sessionStorage.removeItem(this.currentUserKey);
    localStorage.removeItem(this.currentUserKey);
  }

  public static SetUser(pJSONStringify: string) {
    sessionStorage.setItem(this.currentUserKey, pJSONStringify);
    localStorage.removeItem(this.currentUserKey);
  }

  public static GetUser() {
    return this.ParseStorageItem(this.currentUserKey);
  }

  public static GetParishId() {
    const currentUser = this.GetUser() as any;
    return currentUser?.parishId ?? null;
  }

  public static GetCurrentYearActive(): string {
    const currentUser = this.GetUser() as any;
    return currentUser?.yearCurrent ?? '';
  }

  private static ParseStorageItem(storageKey: string) {
    const storageValue = sessionStorage.getItem(storageKey) ?? localStorage.getItem(storageKey);
    if (!storageValue) {
      return null;
    }

    try {
      return JSON.parse(storageValue);
    } catch {
      sessionStorage.removeItem(storageKey);
      localStorage.removeItem(storageKey);
      return null;
    }
  }

}
