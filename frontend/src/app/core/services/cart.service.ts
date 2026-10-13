import { Injectable } from "@angular/core";
import { Subject } from "rxjs";
import { LocalStorageConfig } from "src/app/library/clientconfig/localstorageconfig";
import { GetCourseWithDetailsContent } from "src/app/models/respone_model/course-content-with-detail";

import { TransferHttp } from '../transfer-http/transfer-http';

@Injectable({ providedIn: 'root' })
export class CartServices {
    constructor(private transferHttp: TransferHttp) { }
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
            
            // Sync with backend API
            if (LocalStorageConfig.GetUser()) {
                this.transferHttp.post(`api/v1/Cart/add/${course.id}`, {}).subscribe();
            }
        }
        this.cartUpdates.next('');

    }

    removeCourse(course: GetCourseWithDetailsContent) {
        LocalStorageConfig.RemoveItemOnCart(course.id);
        if (LocalStorageConfig.GetUser()) {
            this.transferHttp.delete(`api/v1/Cart/remove/${course.id}`).subscribe();
        }
        this.cartUpdates.next('');
    }

    removeAllDataOfCart(){
        LocalStorageConfig.RemoveAllDataListCourse();
        if (LocalStorageConfig.GetUser()) {
            this.transferHttp.delete(`api/v1/Cart/clear`).subscribe();
        }
        this.cartUpdates.next('');
    }
}
