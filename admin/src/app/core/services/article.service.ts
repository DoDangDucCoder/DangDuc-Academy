import { Injectable } from "@angular/core";
import { TransferHttp } from "../transfer-http/transfer-http";
import { LinkSettings } from "src/app/library/linksetting/LinkSetting";
import { map } from "rxjs/operators";
import { ArticleModel } from "src/app/models/models/article";
import { RepositoryModel } from "src/app/models/models/repository_base";
import { PagedResult } from "src/app/models/models/paged_result";

@Injectable({ providedIn: 'root' })
export class ArticleService {
    constructor(
        private transferHttp: TransferHttp
    ) { }

    getPagedArticles(pageIndex: number, pageSize: number, search: string = '') {
        const ApiUrl = LinkSettings.GetResLinkSetting('Article', 'GetPagedArticles', pageIndex, pageSize) + (search ? `?search=${search}` : '');
        return this.transferHttp.get(ApiUrl).pipe(map((res: RepositoryModel<PagedResult<ArticleModel>>) => res));
    }

    getArticleById(id: number) {
        const ApiUrl = LinkSettings.GetResLinkSetting('Article', 'GetArticleById', id);
        return this.transferHttp.get(ApiUrl).pipe(map((res: RepositoryModel<ArticleModel>) => res));
    }

    createArticle(model: ArticleModel) {
        const ApiUrl = LinkSettings.GetResLinkSetting('Article', 'CreateArticle');
        return this.transferHttp.post(ApiUrl, model).pipe(map((res: RepositoryModel<ArticleModel>) => res));
    }

    updateArticle(id: number, model: ArticleModel) {
        const ApiUrl = LinkSettings.GetResLinkSetting('Article', 'UpdateArticle', id);
        return this.transferHttp.put(ApiUrl, model).pipe(map((res: RepositoryModel<ArticleModel>) => res));
    }

    deleteArticle(id: number) {
        const ApiUrl = LinkSettings.GetResLinkSetting('Article', 'DeleteArticle', id);
        return this.transferHttp.delete(ApiUrl).pipe(map((res: RepositoryModel<boolean>) => res));
    }
}
