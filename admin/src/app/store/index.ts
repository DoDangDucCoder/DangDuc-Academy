import { ActionReducerMap } from "@ngrx/store";
import { AnalyticsReducer, AnalyticsState } from "./Analytics/analytics.reducer";
import { LearningReducer, LearningState } from "./Learning/learning.reducer";
import { AuthenticationState, authenticationReducer } from "./Authentication/authentication.reducer";
import { LayoutState, layoutReducer } from "./layouts/layout-reducers";
import { InstructorReducer, InstructorState } from "./Learning-instructor/instructor.reducer";
import { StudentsReducer, studentState } from "./students/student.reducer";
import { CourcesReducer, CourcesState } from "./Learning-cources/cources.reducer";

export interface RootReducerState {
    layout: LayoutState,
    auth: AuthenticationState;
    statlist: AnalyticsState;
    Learninglist: LearningState;
    LearningList: InstructorState;
    SubscriptionList: studentState;
    CourcesList: CourcesState;
    Instructorlist: InstructorState
}

export const rootReducer: ActionReducerMap<RootReducerState> = {
    layout: layoutReducer,
    statlist: AnalyticsReducer,
    auth: authenticationReducer,
    Learninglist: LearningReducer,
    LearningList: InstructorReducer,
    SubscriptionList: StudentsReducer,
    CourcesList: CourcesReducer,
    Instructorlist: InstructorReducer
}