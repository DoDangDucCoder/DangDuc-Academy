import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

interface GrammarRule {
  id: number;
  title: string;
  structure: string;
  explanation: string;
  examples: { native: string; trans: string }[];
  tips?: string;
  level: string;
}

interface GrammarSection {
  title: string;
  icon: string;
  rules: GrammarRule[];
}

@Component({
  selector: 'app-language-grammar',
  templateUrl: './language-grammar.component.html',
  styleUrls: ['./language-grammar.component.scss']
})
export class LanguageGrammarComponent implements OnInit {
  lang = 'en';
  selectedSection = 0;
  selectedRule: GrammarRule | null = null;

  languageMeta: Record<string, { name: string; flag: string; color: string }> = {
    en: { name: 'Tiếng Anh', flag: '🇺🇸', color: '#1565c0' },
    zh: { name: 'Tiếng Trung', flag: '🇨🇳', color: '#b71c1c' },
    ja: { name: 'Tiếng Nhật', flag: '🇯🇵', color: '#880e4f' },
    ko: { name: 'Tiếng Hàn', flag: '🇰🇷', color: '#1a237e' },
  };

  grammarData: Record<string, GrammarSection[]> = {
    en: [
      { title: 'Thì Động Từ', icon: '⏰', rules: [
        { id:1, level:'A2', title:'Hiện tại đơn', structure:'S + V(s/es) + O', explanation:'Dùng để diễn tả thói quen, sự thật hiển nhiên, hoặc lịch trình cố định.',
          examples:[{native:'She works every day.',trans:'Cô ấy làm việc mỗi ngày.'},{native:'The sun rises in the east.',trans:'Mặt trời mọc ở phía đông.'}], tips:'Chủ ngữ he/she/it → thêm -s hoặc -es vào động từ.' },
        { id:2, level:'A2', title:'Hiện tại tiếp diễn', structure:'S + am/is/are + V-ing + O', explanation:'Dùng để diễn tả hành động đang xảy ra tại thời điểm nói hoặc tương lai gần.',
          examples:[{native:'I am studying English now.',trans:'Tôi đang học tiếng Anh bây giờ.'},{native:'We are meeting tomorrow.',trans:'Chúng tôi sẽ gặp nhau vào ngày mai.'}] },
        { id:3, level:'B1', title:'Quá khứ đơn', structure:'S + V(ed/2) + O', explanation:'Dùng để diễn tả hành động đã xảy ra và kết thúc trong quá khứ.',
          examples:[{native:'She visited Paris last year.',trans:'Cô ấy đã thăm Paris năm ngoái.'},{native:'He didn\'t come to the party.',trans:'Anh ấy đã không đến bữa tiệc.'}], tips:'Dạng phủ định: did not + V nguyên thể.' },
        { id:4, level:'B2', title:'Hiện tại hoàn thành', structure:'S + have/has + V(p.p) + O', explanation:'Dùng để nói về trải nghiệm, kết quả ảnh hưởng đến hiện tại, hoặc hành động vừa xảy ra.',
          examples:[{native:'I have lived in Hanoi for 5 years.',trans:'Tôi đã sống ở Hà Nội được 5 năm.'},{native:'She has already finished the report.',trans:'Cô ấy đã hoàn thành báo cáo rồi.'}], tips:'Thường đi với: already, just, yet, ever, never, for, since.' },
      ]},
      { title: 'Câu Điều Kiện', icon: '🔀', rules: [
        { id:5, level:'B1', title:'Loại 0 — Sự thật hiển nhiên', structure:'If + S + V(present), S + V(present)', explanation:'Dùng cho những sự thật luôn luôn đúng, quy luật tự nhiên.',
          examples:[{native:'If you heat water to 100°C, it boils.',trans:'Nếu bạn đun nước đến 100°C, nó sẽ sôi.'}] },
        { id:6, level:'B1', title:'Loại 1 — Điều kiện có thể xảy ra', structure:'If + S + V(present), S + will + V', explanation:'Dùng khi điều kiện có khả năng xảy ra trong thực tế.',
          examples:[{native:'If it rains, I will stay home.',trans:'Nếu trời mưa, tôi sẽ ở nhà.'}] },
        { id:7, level:'B2', title:'Loại 2 — Giả thuyết hiện tại', structure:'If + S + V(past), S + would + V', explanation:'Dùng để diễn tả tình huống không có thật hoặc khó xảy ra trong hiện tại.',
          examples:[{native:'If I were a bird, I would fly to you.',trans:'Nếu tôi là chim, tôi sẽ bay đến chỗ bạn.'}], tips:'Dùng "were" cho tất cả chủ ngữ trong loại 2.' },
      ]},
      { title: 'Giới Từ', icon: '📌', rules: [
        { id:8, level:'A2', title:'Giới từ chỉ thời gian', structure:'at / on / in + thời gian', explanation:'AT: giờ cụ thể, ON: ngày/thứ, IN: tháng/năm/buổi.',
          examples:[{native:'at 8 o\'clock, on Monday, in January',trans:'lúc 8 giờ, vào thứ Hai, vào tháng Một'},{native:'I was born in 1999.',trans:'Tôi sinh năm 1999.'}] },
        { id:9, level:'B1', title:'Giới từ chỉ nơi chốn', structure:'at / on / in + nơi chốn', explanation:'IN: bên trong, ON: trên bề mặt, AT: tại điểm cụ thể.',
          examples:[{native:'The book is on the table.',trans:'Cuốn sách ở trên bàn.'},{native:'She is at the airport.',trans:'Cô ấy đang ở sân bay.'}] },
      ]},
    ],
    ja: [
      { title: 'Trợ Từ (助詞)', icon: '📝', rules: [
        { id:1, level:'N5', title:'は (wa) — Chủ đề câu', structure:'[Chủ đề] は ～', explanation:'Đánh dấu chủ đề của câu, không nhất thiết là chủ ngữ ngữ pháp.',
          examples:[{native:'わたし は がくせい です。',trans:'(Về tôi thì,) Tôi là học sinh.'},{native:'きょう は いいてんき ですね。',trans:'Hôm nay thời tiết đẹp nhỉ.'}] },
        { id:2, level:'N5', title:'が (ga) — Chủ ngữ', structure:'[Chủ ngữ] が ～', explanation:'Đánh dấu chủ ngữ ngữ pháp, nhấn mạnh vào chủ ngữ.',
          examples:[{native:'だれ が きましたか？ —— たなかさん が きました。',trans:'Ai đến? — Tanaka đến.'},{native:'ねこ が います。',trans:'Có con mèo (ở đây).'}] },
        { id:3, level:'N5', title:'を (wo) — Tân ngữ trực tiếp', structure:'[Tân ngữ] を + ngoại động từ', explanation:'Đánh dấu đối tượng chịu tác động của hành động.',
          examples:[{native:'りんご を たべます。',trans:'Tôi ăn táo.'},{native:'にほんご を べんきょう します。',trans:'Tôi học tiếng Nhật.'}] },
        { id:4, level:'N5', title:'に (ni) — Hướng / Thời gian / Vị trí', structure:'[Nơi/Thời gian] に ～', explanation:'Dùng cho đích đến, thời điểm cụ thể, hoặc nơi tồn tại.',
          examples:[{native:'がっこう に いきます。',trans:'Tôi đi đến trường.'},{native:'3じ に あいましょう。',trans:'Gặp nhau lúc 3 giờ nhé.'}] },
      ]},
      { title: 'Mẫu Câu Cơ Bản', icon: '💬', rules: [
        { id:5, level:'N5', title:'～です / ～ます', structure:'名詞 + です / 動詞 + ます', explanation:'"です" dùng với danh từ/tính từ. "ます" dùng với động từ (lịch sự, hiện tại/tương lai).',
          examples:[{native:'これ は ほん です。',trans:'Đây là cuốn sách.'},{native:'まいにち べんきょう します。',trans:'Tôi học hàng ngày.'}] },
        { id:6, level:'N5', title:'～ません / ～じゃないです', structure:'V + ません / N + じゃないです', explanation:'Dạng phủ định lịch sự.',
          examples:[{native:'いきません。',trans:'Tôi không đi.'},{native:'がくせい じゃないです。',trans:'Tôi không phải học sinh.'}] },
      ]},
    ],
    zh: [
      { title: 'Cấu Trúc Cơ Bản', icon: '📐', rules: [
        { id:1, level:'HSK1', title:'Câu khẳng định — 是 (shì)', structure:'主语 + 是 + 宾语', explanation:'"是" (shì) = là, dùng để định nghĩa hoặc giải thích.',
          examples:[{native:'我是学生。',trans:'Wǒ shì xuéshēng. → Tôi là học sinh.'},{native:'他是老师。',trans:'Tā shì lǎoshī. → Anh ấy là giáo viên.'}] },
        { id:2, level:'HSK1', title:'Câu phủ định — 不 (bù)', structure:'主语 + 不 + 动词/形容词', explanation:'"不" (bù) phủ định động từ và tính từ; "没" (méi) phủ định 有 hoặc quá khứ.',
          examples:[{native:'我不是中国人。',trans:'Wǒ bù shì Zhōngguó rén. → Tôi không phải người Trung Quốc.'},{native:'我不去。',trans:'Wǒ bù qù. → Tôi không đi.'}] },
        { id:3, level:'HSK1', title:'Câu hỏi — 吗 (ma)', structure:'陈述句 + 吗？', explanation:'Thêm "吗" vào cuối câu khẳng định để tạo câu hỏi.',
          examples:[{native:'你是学生吗？',trans:'Nǐ shì xuéshēng ma? → Bạn là học sinh phải không?'},{native:'你吃饭了吗？',trans:'Nǐ chī fàn le ma? → Bạn ăn cơm chưa?'}] },
      ]},
      { title: 'Lượng Từ (量词)', icon: '🔢', rules: [
        { id:4, level:'HSK1', title:'Lượng từ 个 (gè)', structure:'数词 + 个 + 名词', explanation:'"个" là lượng từ thông dụng nhất, dùng cho người và nhiều đồ vật.',
          examples:[{native:'一个人 / 两个苹果',trans:'yī gè rén / liǎng gè píngguǒ → một người / hai quả táo'}] },
        { id:5, level:'HSK2', title:'Các lượng từ đặc biệt', structure:'数词 + [量词] + 名词', explanation:'Mỗi danh từ có lượng từ riêng: 本(sách), 张(giấy/bàn), 条(cá/đường)..',
          examples:[{native:'一本书 / 一张纸',trans:'yī běn shū / yī zhāng zhǐ → một cuốn sách / một tờ giấy'}] },
      ]},
    ],
    ko: [
      { title: 'Trợ Từ Chủ Đề & Chủ Ngữ', icon: '📝', rules: [
        { id:1, level:'TOPIK-I', title:'은/는 — Trợ từ chủ đề', structure:'[Danh từ] 은/는 ～', explanation:'"은" sau phụ âm, "는" sau nguyên âm. Đánh dấu chủ đề câu.',
          examples:[{native:'저는 학생이에요.',trans:'Tôi là học sinh.'},{native:'오늘은 날씨가 좋아요.',trans:'Hôm nay trời đẹp.'}] },
        { id:2, level:'TOPIK-I', title:'이/가 — Trợ từ chủ ngữ', structure:'[Danh từ] 이/가 ～', explanation:'"이" sau phụ âm, "가" sau nguyên âm. Đánh dấu chủ ngữ ngữ pháp.',
          examples:[{native:'누가 왔어요? — 친구가 왔어요.',trans:'Ai đến? — Bạn tôi đến.'},{native:'무엇이 맛있어요?',trans:'Cái gì ngon?'}] },
        { id:3, level:'TOPIK-I', title:'을/를 — Trợ từ tân ngữ', structure:'[Danh từ] 을/를 + 동사', explanation:'"을" sau phụ âm, "를" sau nguyên âm. Đánh dấu tân ngữ.',
          examples:[{native:'밥을 먹어요.',trans:'Tôi ăn cơm.'},{native:'한국어를 공부해요.',trans:'Tôi học tiếng Hàn.'}] },
      ]},
      { title: 'Đuôi Động Từ', icon: '🔧', rules: [
        { id:4, level:'TOPIK-I', title:'아/어요 — Thể lịch sự thông thường', structure:'동사 어근 + 아요/어요', explanation:'Khi nguyên âm cuối là ㅏ/ㅗ → 아요, các trường hợp còn lại → 어요.',
          examples:[{native:'가다 → 가요 / 먹다 → 먹어요',trans:'đi → đi (lịch sự) / ăn → ăn (lịch sự)'},{native:'공부하다 → 공부해요',trans:'học → học (lịch sự)'}] },
        { id:5, level:'TOPIK-I', title:'이에요/예요 — Danh từ + là', structure:'N + 이에요 (phụ âm) / 예요 (nguyên âm)', explanation:'Dùng để nói "là + danh từ" theo thể lịch sự thông thường.',
          examples:[{native:'학생이에요. / 의사예요.',trans:'Là học sinh. / Là bác sĩ.'}] },
      ]},
    ],
  };

  get meta() { return this.languageMeta[this.lang] ?? this.languageMeta['en']; }
  get sections() { return this.grammarData[this.lang] ?? []; }
  get currentSection() { return this.sections[this.selectedSection]; }

  constructor(private route: ActivatedRoute) {}

  ngOnInit(): void {
    this.lang = this.route.snapshot.params['lang'] || 'en';
  }

  selectSection(i: number): void {
    this.selectedSection = i;
    this.selectedRule = null;
  }

  selectRule(rule: GrammarRule): void {
    this.selectedRule = this.selectedRule?.id === rule.id ? null : rule;
  }

  getLevelColor(level: string): string {
    const map: Record<string, string> = {
      'A1':'#43a047','A2':'#66bb6a','B1':'#1e88e5','B2':'#8e24aa',
      'C1':'#e53935','N5':'#43a047','N4':'#66bb6a','N3':'#ffa726','N2':'#ef5350','N1':'#e53935',
      'HSK1':'#43a047','HSK2':'#66bb6a','HSK3':'#1e88e5','TOPIK-I':'#43a047','TOPIK-II':'#8e24aa'
    };
    return map[level] ?? '#757575';
  }
}
