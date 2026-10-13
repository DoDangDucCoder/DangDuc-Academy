export interface ArticleModel {
    id: number;
    title: string;
    slug?: string;
    description: string;
    content: string;
    thumbnailUrl: string;
    authorId?: number;
    authorName?: string;
    viewCount?: number;
    createDate?: string;
    updateDate?: string;
}
