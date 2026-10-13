import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

interface LanguageCard {
  code: string;
  name: string;
  flag: string;
  types: string;
  gradientFrom: string;
  gradientTo: string;
  examCount: number;
  vocabCount: number;
}

@Component({
  selector: 'app-language-hub',
  templateUrl: './language-hub.component.html',
  styleUrls: ['./language-hub.component.scss']
})
export class LanguageHubComponent implements OnInit {

  languages: LanguageCard[] = [
    { code: 'en', name: 'Tiếng Anh',  flag: '🇺🇸', types: 'TOEIC · IELTS · TOEFL',
      gradientFrom: '#1565c0', gradientTo: '#1976d2', examCount: 2847, vocabCount: 50000 },
    { code: 'zh', name: 'Tiếng Trung', flag: '🇨🇳', types: 'HSK 1–6',
      gradientFrom: '#b71c1c', gradientTo: '#c62828', examCount: 1200, vocabCount: 20000 },
    { code: 'ja', name: 'Tiếng Nhật',  flag: '🇯🇵', types: 'JLPT N5–N1',
      gradientFrom: '#880e4f', gradientTo: '#ad1457', examCount: 980,  vocabCount: 15000 },
    { code: 'ko', name: 'Tiếng Hàn',  flag: '🇰🇷', types: 'TOPIK I–II',
      gradientFrom: '#1a237e', gradientTo: '#283593', examCount: 750,  vocabCount: 12000 },
  ];

  features = [
    { icon: '📝', title: 'Đề Thi Thử', desc: 'Làm bài theo chuẩn quốc tế, có đồng hồ đếm ngược, chấm điểm tự động và giải thích đáp án chi tiết.' },
    { icon: '📚', title: 'Flashcard SRS', desc: 'Học từ vựng qua thẻ ghi nhớ thông minh theo thuật toán SM-2 — đúng khi nào cần ôn.' },
    { icon: '🏆', title: 'Bảng Xếp Hạng', desc: 'Thi đua với hàng nghìn học viên khác, xem thứ hạng của bạn sau mỗi bài thi.' },
  ];

  stats = [
    { value: '150,000+', label: 'Học viên' },
    { value: '5,500+', label: 'Đề thi' },
    { value: '97,000+', label: 'Từ vựng' },
    { value: '4', label: 'Ngôn ngữ' },
  ];

  constructor(private router: Router) {}

  ngOnInit(): void {}

  goToLanguage(code: string): void {
    this.router.navigate(['/language', code]);
  }
}
