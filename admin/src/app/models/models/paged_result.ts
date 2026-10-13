export interface PagedResult<T> {
    results: T[];
    currentPage: number;
    pageCount: number;
    pageSize: number;
    rowCount: number;
    firstRowOnPage?: number;
    lastRowOnPage?: number;
}
