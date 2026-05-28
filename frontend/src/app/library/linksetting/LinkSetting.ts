import resLinkSetting from '../../../assets/document/reslink-api.json';
import { String } from '../share-function/string';

export class LinkSettings {
    public static GetResLinkSetting(pGroup: string, pFunction: string, ...pParams: any[]) {
        const link = (resLinkSetting as any)?.[pGroup]?.[pFunction];
        return String.Format(link, this.normalizeParams(pParams));
    }

    private static normalizeParams(params: any[]): string[] {
        return params.map((param) => this.encodePathSegment(param));
    }

    private static encodePathSegment(value: any): string {
        if (value === null || value === undefined) {
            return String.Empty;
        }

        return encodeURIComponent(`${value}`);
    }
}
