import { Injectable } from "@angular/core";
import { Subject } from "rxjs";
import { LocalStorageConfig } from "src/app/library/clientconfig/localstorageconfig";
import { GetCourseWithDetailsContent } from "src/app/models/respone_model/course-content-with-detail";

@Injectable({ providedIn: 'root' })
export class CartServices {
    constructor() { }
    private cartUpdates = new Subject<string>();
    public cartUpdates$ = this.cartUpdates.asObservable();
    public get courseItems(): GetCourseWithDetailsContent[] {
        return LocalStorageConfig.GetListCourseAddCart() ?? Array<GetCourseWithDetailsContent>();
    }

    public get count(): number {
        return LocalStorageConfig.GetListCourseAddCart()?.length ?? 0;
    };


    addCourse(course: GetCourseWithDetailsContent) {
        const item = LocalStorageConfig.GetListCourseAddCart()?.find(item => item.id === course.id);
        if (item) { return; } else {
            course.totalOrder = course.isDiscount == true ? course.priceOfDiscount : course.priceOfCourse;
            const getCourses = LocalStorageConfig.GetListCourseAddCart() ?? Array<GetCourseWithDetailsContent>();
            getCourses.push(course);
            LocalStorageConfig.AddListCourses(getCourses);
        }
        this.cartUpdates.next('');

    }

    removeCourse(course: GetCourseWithDetailsContent) {
        LocalStorageConfig.RemoveItemOnCart(course.id);
        this.cartUpdates.next('');
    }

    removeAllDataOfCart(){
        LocalStorageConfig.RemoveAllDataListCourse();
        this.cartUpdates.next('');
    }
}
