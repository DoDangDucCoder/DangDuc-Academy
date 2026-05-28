import { Injectable } from "@angular/core";
import { map } from 'rxjs/operators';
import { Actions, createEffect, ofType } from "@ngrx/effects";
import { feedback, property, rentproperty, saleproperty } from "src/app/core/data";
import { fetchfeedbackdataData, fetchfeedbackdataSuccess, fetchpropertydataData, fetchpropertydataSuccess, fetchrentproprtydataData, fetchrentproprtydataSuccess, fetchsalepropertydataData, fetchsalepropertydataSuccess } from "./realEstate.action";


@Injectable()
export class RealEffects {
    fetchData$ = createEffect(() =>
        this.actions$.pipe(
            ofType(fetchpropertydataData),
            map(() => fetchpropertydataSuccess({ propertydata: property }))
        )
    );

    fetchrecentData$ = createEffect(() =>
        this.actions$.pipe(
            ofType(fetchfeedbackdataData),
            map(() => fetchfeedbackdataSuccess({ feedbackdata: feedback }))
        )
    );
    fetchsalepropertyData$ = createEffect(() =>
        this.actions$.pipe(
            ofType(fetchsalepropertydataData),
            map(() => fetchsalepropertydataSuccess({ salepropertydata: saleproperty }))
        )
    );
    fetchrecentcoursetData$ = createEffect(() =>
        this.actions$.pipe(
            ofType(fetchrentproprtydataData),
            map(() => fetchrentproprtydataSuccess({ rentproprtydata: rentproperty }))
        )
    );

    constructor(private actions$: Actions) { }
} 
