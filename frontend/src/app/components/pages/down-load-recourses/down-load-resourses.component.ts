import { Component, OnInit } from '@angular/core';

@Component({
    selector: 'app-down-load-recourses',
    templateUrl: './down-load-recourses.component.html',
    styleUrls: ['./down-load-recourses.component.scss']
})
export class DownLoadRecoursesComponent implements OnInit {

    documents = [
        { title: 'Phần mềm AutoCAD 2024 Full Crack', date: '16/06/2026', image: 'https://images.unsplash.com/photo-1581092921461-eab62e97a780?auto=format&fit=crop&w=700&q=80', tab: 'tab2' },
        { title: 'SolidWorks 2023 Premium', date: '15/06/2026', image: 'https://images.unsplash.com/photo-1537202108838-e7072bad1927?auto=format&fit=crop&w=700&q=80', tab: 'tab2' },
        { title: 'Visual Studio Code & Git', date: '14/06/2026', image: 'https://images.unsplash.com/photo-1618401471353-b98afee0b2eb?auto=format&fit=crop&w=700&q=80', tab: 'tab2' },
        { title: 'Bản vẽ 3D Động cơ V8 (SolidWorks)', date: '16/06/2026', image: 'https://images.unsplash.com/photo-1581092160607-ee22621dd758?auto=format&fit=crop&w=700&q=80', tab: 'tab3' },
        { title: 'Bản vẽ 2D Biệt thự Tân cổ điển (AutoCAD)', date: '10/06/2026', image: 'https://images.unsplash.com/photo-1503387762-592deb58ef4e?auto=format&fit=crop&w=700&q=80', tab: 'tab3' },
        { title: 'Tiêu chuẩn kích thước khuôn ép nhựa', date: '12/06/2026', image: 'https://images.unsplash.com/photo-1581092787765-e3feb951d987?auto=format&fit=crop&w=700&q=80', tab: 'tab4' },
        { title: 'Tài liệu đo lường dung sai lắp ghép', date: '05/06/2026', image: 'https://images.unsplash.com/photo-1504917595217-d4dc5ebe6122?auto=format&fit=crop&w=700&q=80', tab: 'tab4' },
        { title: 'Sổ tay gia công phay CNC Mastercam', date: '16/06/2026', image: 'https://images.unsplash.com/photo-1516321318423-f06f85e504b3?auto=format&fit=crop&w=700&q=80', tab: 'tab5' },
        { title: 'Giáo trình lập trình tiện CNC G-Code', date: '11/06/2026', image: 'https://images.unsplash.com/photo-1454165804606-c3d57bc86b40?auto=format&fit=crop&w=700&q=80', tab: 'tab5' }
    ];

    constructor() { }

    ngOnInit(): void {}

    // Tabs
    currentTab = 'tab1';
    switchTab(event: MouseEvent, tab: string) {
        event.preventDefault();
        this.currentTab = tab;
    }

    getDocumentsByTab(tabId: string) {
        if (tabId === 'tab1') return this.documents;
        return this.documents.filter(d => d.tab === tabId);
    }

}