import { StudentUser } from "src/app/models/models/student-user";
import { GetCourseWithDetailsContent } from "src/app/models/respone_model/course-content-with-detail";
import { LoginRegister } from "src/app/models/respone_model/login-register-respone";

export class LocalStorageConfig {

  private static currentUserKey = 'currentUser';
  private static listCourseId = 'listCourse';

  public static RemoveUser(key: string) {
    sessionStorage.removeItem(key);
    localStorage.removeItem(key);
  }

  public static SetUser(pJSONStringify: LoginRegister) {
    const valueUpdate = JSON.stringify(pJSONStringify);
    sessionStorage.setItem(this.currentUserKey, valueUpdate);
    localStorage.removeItem(this.currentUserKey);
  }

  public static GetUser(): LoginRegister | null {
    return this.ParseStorageItem<LoginRegister>(this.currentUserKey);
  }

  public static GetCurrentYearActive(): string {
    const currentUser = this.GetUser() as any;
    return currentUser?.yearCurrent ?? '';
  }

  public static AddListCourses(listCourses: GetCourseWithDetailsContent[]) {
    this.RemoveUser(this.listCourseId);
    const valueUpdate = JSON.stringify(listCourses);
    localStorage.setItem(this.listCourseId, valueUpdate);
  }

  public static GetListCourseAddCart(): GetCourseWithDetailsContent[] | null {
    return this.ParseStorageItem<GetCourseWithDetailsContent[]>(this.listCourseId);
  }

  public static RemoveItemOnCart(id: number) {
    const listCourse = this.GetListCourseAddCart();
    if (listCourse === null) return;
    let items = listCourse.filter(item => item.id !== id);
    this.RemoveUser(this.listCourseId);
    this.AddListCourses(items);
  }

  public static RemoveAllDataListCourse(){
    this.RemoveUser(this.listCourseId);
  }

  private static ParseStorageItem<T>(storageKey: string): T | null {
    const storageValue = sessionStorage.getItem(storageKey) ?? localStorage.getItem(storageKey);
    if (!storageValue) {
      return null;
    }

    try {
      return JSON.parse(storageValue) as T;
    } catch {
      sessionStorage.removeItem(storageKey);
      localStorage.removeItem(storageKey);
      return null;
    }
  }

}
