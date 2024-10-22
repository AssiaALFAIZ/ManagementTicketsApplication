// PagedResult.ts

export interface PagedResult<T> {
    records: T[];
    page: number;
    totalRecords: number;
    totalPages: number;
}
