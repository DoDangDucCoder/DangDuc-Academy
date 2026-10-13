import { AfterViewInit, Component, ElementRef, OnInit, OnDestroy, ViewChild, inject } from '@angular/core';
import "@mux/mux-player";
import { BeforeSlideDetail } from 'lightgallery/lg-events';
import lgVideo from 'lightgallery/plugins/video';
import { UserCommentCoursesComponent } from '../../dialog/user-comment-courses/user-comment-courses.component';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { UserReviewCoursesComponent } from '../../dialog/user-review-courses/user-review-courses.component';
import Artplayer from 'artplayer';
import { type Option } from 'artplayer/types/option';
import Hls from 'hls.js';
import { CourseServices } from 'src/app/core/services/course.service';
import { MessengerServices } from 'src/app/core/services/messenger.service';
import { ActivatedRoute } from '@angular/router';
import { GetCourseWithDetailsContent } from 'src/app/models/respone_model/course-content-with-detail';
import { CourseContentDetails } from 'src/app/models/models/course-content';
import { CourseEvaluation } from 'src/app/models/models/course-evaluation';
import { ConstantValue } from 'src/app/models/contants/ennum_router';
import { LocalStorageConfig } from 'src/app/library/clientconfig/localstorageconfig';
import { TheadQuestionCourse } from 'src/app/models/models/thead-question-course';
import { StudentServices } from 'src/app/core/services/student.service';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { CourseNoteService, CourseNoteModel } from 'src/app/core/services/course-note.service';
import { SubtitleService, SubtitleModel } from 'src/app/core/services/subtitle.service';
import { AssignmentService, Assignment, AssignmentSubmission } from 'src/app/core/services/assignment.service';
import { ConfettiService } from 'src/app/core/services/confetti.service';
import { VideoPlaybackService } from 'src/app/core/services/video-playback.service';
import { VideoEngagementService } from 'src/app/core/services/video-engagement.service';
import { LessonCompletionService } from 'src/app/core/services/lesson-completion.service';
import { CertificateService } from 'src/app/core/services/certificate.service';


@Component({
  selector: 'app-courses-content-detail',
  templateUrl: './courses-content-detail.component.html',
  styleUrls: ['./courses-content-detail.component.scss'],
  host: { class: 'd-block' },
})
export class CoursesContentDetailComponent implements OnInit, AfterViewInit, OnDestroy {

  // Keyboard shortcuts handler
  private keyboardHandler = (e: KeyboardEvent) => {
    if (!this.player || !this.player.video) return;
    // Don't capture keys when typing in inputs
    const tag = (e.target as HTMLElement)?.tagName;
    if (tag === 'INPUT' || tag === 'TEXTAREA' || tag === 'SELECT') return;

    switch (e.key) {
      case ' ':
        e.preventDefault();
        this.player.video.paused ? this.player.play() : this.player.pause();
        break;
      case 'ArrowLeft':
        e.preventDefault();
        this.player.video.currentTime = Math.max(0, this.player.video.currentTime - 5);
        break;
      case 'ArrowRight':
        e.preventDefault();
        this.player.video.currentTime = Math.min(this.player.video.duration, this.player.video.currentTime + 5);
        break;
      case 'ArrowUp':
        e.preventDefault();
        this.player.video.volume = Math.min(1, this.player.video.volume + 0.1);
        break;
      case 'ArrowDown':
        e.preventDefault();
        this.player.video.volume = Math.max(0, this.player.video.volume - 0.1);
        break;
      case 'f':
      case 'F':
        this.player.fullscreen = !this.player.fullscreen;
        break;
      case 'm':
      case 'M':
        this.player.video.muted = !this.player.video.muted;
        break;
    }
  };

  private modalService = inject(NgbModal);
  isDisplayReplay = false;
  isComment = false;
  @ViewChild('artplayer') artplayerElement!: ElementRef;
  player: Artplayer | undefined;
  playerHeight = '100%';
  option: Option = {
    container: '',
    url: '',
    volume: 0.5,
    isLive: false,
    muted: false,
    autoplay: true,
    pip: true,
    autoMini: true,
    setting: true,
    loop: true,
    flip: true,
    playbackRate: true,
    aspectRatio: true,
    fullscreen: true,
    fullscreenWeb: true,
    subtitleOffset: true,
    miniProgressBar: true,
    mutex: true,
    backdrop: true,
    playsInline: true,
    autoPlayback: true,
    airplay: true,
    theme: '#23ade5',
    lang: navigator.language.toLowerCase(),
    hotkey: true,
    type: 'm3u8',
    subtitle: {
      url: '',
      type: 'vtt',
      style: {
          color: '#fff',
      },
      encoding: 'utf-8',
    },
    customType: {
      m3u8: function playM3u8(video, url, art) {
        if (Hls.isSupported()) {
          if (art.hls) art.hls.destroy();
          const hls = new Hls();
          hls.loadSource(url);
          hls.attachMedia(video);
          art.hls = hls;
          art.on('destroy', () => hls.destroy());
        } else if (video.canPlayType('application/vnd.apple.mpegurl')) {
          video.src = url;
        } else {
          art.notice.show = 'Unsupported playback format: m3u8';
        }
      }
    },
  }
  idCourse = 0;
  contentCourseDetail: GetCourseWithDetailsContent | null | undefined;
  videoOfContent: CourseContentDetails;
  activeItemIndex: number | null = null;
  commentOfCourse: CourseEvaluation[] = [];
  theadQuestionOfCourse: TheadQuestionCourse[] = [];
  listNameRegister: string[] = [];
  youtubeEmbedUrl?: SafeResourceUrl;
  notes: CourseNoteModel[] = [];
  newNoteContent: string = '';
  subtitles: SubtitleModel[] = [];

  // Assignments
  assignments: Assignment[] = [];
  isLoadingAssignments = false;
  selectedAssignment: Assignment | null = null;
  submissionText = '';
  submissionFileUrl = '';
  mySubmission: AssignmentSubmission | null = null;

  // Auto-advance countdown
  showCountdown = false;
  countdownSeconds = 5;
  private countdownTimer: any;

  // Auto-save
  private autoSaveInterval: any;

  // Video position sync
  private positionSaveInterval: any;

  // Course progress percentage
  courseProgress = 0;

  // Certificate
  isCertLoading = false;

  // Sidebar toggle — open by default on desktop
  sidebarOpen = true;
  toggleSidebar() {
    this.sidebarOpen = !this.sidebarOpen;
  }

  // Role check — Teacher=3, Admin=0
  get isInstructorOrAdmin(): boolean {
    const user = LocalStorageConfig.GetUser();
    return user?.roleId === 3 || user?.roleId === 0;
  }

  // Course Notes UI state (alias for existing notes array + new UI props)
  get courseNotes() { return this.notes; }
  newNoteTimestamp = 0;
  isLoadingNotes = false;
  isAddingNote = false;

  constructor(
    private readonly courseServices: CourseServices,
    private readonly messengerService: MessengerServices,
    private readonly router: ActivatedRoute,
    private readonly studentServices: StudentServices,
    private readonly sanitizer: DomSanitizer,
    private readonly courseNoteService: CourseNoteService,
    private readonly subtitleService: SubtitleService,
    private readonly assignmentService: AssignmentService,
    private readonly confettiService: ConfettiService,
    private readonly videoPlaybackService: VideoPlaybackService,
    private readonly videoEngagementService: VideoEngagementService,
    private readonly lessonCompletionService: LessonCompletionService,
    private readonly certificateService: CertificateService
  ) { }


  ngOnInit() {
    this.idCourse = Number(this.router.snapshot.paramMap.get('id'));
    this.loadDataForCourse(this.idCourse);
    this.getListCommentOfCourse(this.idCourse);
    this.getListTheadQuestionOfCourse(this.idCourse);
    this.getListUserNameRegisterForCourse(this.idCourse);
    this.loadAssignments();
    this.restoreDraft();
    this.startAutoSave();
    document.addEventListener('keydown', this.keyboardHandler);
  }

  ngAfterViewInit(): void {
    this.option.container = this.artplayerElement.nativeElement;
    if (this.artplayerElement) {
      this.player = new Artplayer(this.option);
      // Auto-advance: listen for video ended event
      this.player.on('video:ended', () => {
        this.onVideoEnded();
        this.videoEngagementService.trackEvent({
          idContentDetail: this.activeItemIndex ?? 0,
          eventType: 'ended',
          videoPosition: this.player?.video?.currentTime ?? 0,
          videoDuration: this.player?.video?.duration ?? 0
        });
      });
      // Engagement: track play/pause/seek
      this.player.on('play', () => {
        this.videoEngagementService.trackEvent({
          idContentDetail: this.activeItemIndex ?? 0,
          eventType: 'play',
          videoPosition: this.player?.video?.currentTime ?? 0,
          videoDuration: this.player?.video?.duration ?? 0
        });
      });
      this.player.on('pause', () => {
        this.videoEngagementService.trackEvent({
          idContentDetail: this.activeItemIndex ?? 0,
          eventType: 'pause',
          videoPosition: this.player?.video?.currentTime ?? 0,
          videoDuration: this.player?.video?.duration ?? 0
        });
      });
      this.player.on('seek', () => {
        this.videoEngagementService.trackEvent({
          idContentDetail: this.activeItemIndex ?? 0,
          eventType: 'seek',
          videoPosition: this.player?.video?.currentTime ?? 0,
          videoDuration: this.player?.video?.duration ?? 0
        });
      });
      // Cross-device position sync: save every 10s
      this.positionSaveInterval = setInterval(() => {
        if (this.player && this.player.video && this.activeItemIndex && !this.player.video.paused) {
          this.videoPlaybackService.savePosition(
            this.activeItemIndex,
            this.player.video.currentTime,
            this.player.video.duration
          ).subscribe();
        }
      }, 10000);
      this.videoEngagementService.startTracking();
    }
    this.updatePlayerUrl(this.getVideoContentUrl() ?? this.getCourseVideoUrl());
  }

  selectedVideoUrl: string;

  selectCourse(course: { videoUrl: string; }) {
    this.selectedVideoUrl = course.videoUrl;
  }

  settings = {
    counter: false,
    plugins: [lgVideo]
  };
  onBeforeSlide = (detail: BeforeSlideDetail): void => {
    const { index, prevIndex } = detail;
    console.log(index, prevIndex);
  };

  // Tabs
  currentTab = 'tab1';
  switchTab(event: Event, tab: string) {
    event.preventDefault();
    this.currentTab = tab;
  }

  openModalCommentUser() {
    const modalRef = this.modalService.open(UserCommentCoursesComponent, { size: 'xl' });
    modalRef.componentInstance.name = 'World';
  }

  openModalReviewCourses() {
    const getValueLocalStor = LocalStorageConfig.GetUser();
    if (!getValueLocalStor) {
      this.messengerService.warringWithIssue();
      return;
    }

    const modalRef = this.modalService.open(UserReviewCoursesComponent, { size: 'xl' });
    modalRef.componentInstance.reviewOfUser = '';
    modalRef.componentInstance.idCourse = this.idCourse;
    modalRef.componentInstance.idStudent = getValueLocalStor.userId;
    modalRef.closed.subscribe((res) => {
      if (res == ConstantValue.finishModal) {
        this.getListCommentOfCourse(this.idCourse);
        this.loadDataForCourse(this.idCourse);
      } else {
        this.messengerService.errorWithIssue();
      }
    })
  }

  replayComment() {
    this.isDisplayReplay = true;
  }

  getDetailDataOfCourse() {

  }

  loadDataForCourse(id: number) {
    this.courseServices.getCourses(id).subscribe((res) => {
      if (res.retCode == 0 && res.systemMessage == "") {
        this.contentCourseDetail = res.data;
        this.updatePlayerUrl(this.getCourseVideoUrl());
        this.calculateProgress();
      } else {
        this.messengerService.errorWithIssue();
      }
    });
  }

  getDataOfContentDetails(id: number) {
    this.courseServices.getContentCourseDetails(id).subscribe((res) => {
      if (res.retCode == 0 && res.systemMessage == "") {
        this.videoOfContent = res.data;
        this.subtitleService.getSubtitles(id).subscribe((subRes) => {
          if (subRes.statusCode == 200 && subRes.data) {
            this.subtitles = subRes.data;
          } else {
            this.subtitles = [];
          }
          this.updatePlayerUrl(this.getVideoContentUrl());
        });
      } else {
        this.messengerService.errorWithIssue();
      }
    });
  }

  // --- Assignments ---
  loadAssignments() {
    this.isLoadingAssignments = true;
    this.assignmentService.getAssignmentsByCourse(this.idCourse).subscribe({
      next: (res) => {
        this.assignments = res.data ?? [];
        this.isLoadingAssignments = false;
      },
      error: () => {
        this.assignments = [];
        this.isLoadingAssignments = false;
      }
    });
  }

  viewAssignment(a: Assignment) {
    this.selectedAssignment = a;
    this.submissionText = '';
    this.submissionFileUrl = '';
    this.mySubmission = null;
    
    const user = LocalStorageConfig.GetUser();
    if (user && user.userId) {
      this.isLoadingAssignments = true;
      this.assignmentService.getSubmissionByUserAndAssignment(a.id!, user.userId).subscribe({
        next: (res) => {
          this.mySubmission = res.data;
          this.isLoadingAssignments = false;
        },
        error: () => {
          this.mySubmission = null;
          this.isLoadingAssignments = false;
        }
      });
    }
  }

  submitAssignment() {
    if (!this.selectedAssignment) return;
    const user = LocalStorageConfig.GetUser();
    if (!user || !user.userId) {
      this.messengerService.warringWithIssue();
      return;
    }

    this.isLoadingAssignments = true;
    const sub: AssignmentSubmission = {
      idAssignment: this.selectedAssignment.id!,
      idUser: user.userId,
      submissionText: this.submissionText,
      fileUrl: this.submissionFileUrl
    };

    this.assignmentService.submitAssignment(sub).subscribe({
      next: () => {
        this.messengerService.successes('Submitted successfully');
        this.confettiService.celebrate();
        this.clearDraft();
        this.viewAssignment(this.selectedAssignment!); // Reload my submission
      },
      error: () => {
        this.messengerService.errorWithIssue();
        this.isLoadingAssignments = false;
      }
    });
  }

  selectContentCourse(id: number) {
    this.activeItemIndex = id;
    this.getDataOfContentDetails(id);
    // Restore saved playback position for cross-device resume
    this.videoPlaybackService.getPosition(id).subscribe({
      next: (res: any) => {
        if (res.data && res.data > 0 && this.player) {
          setTimeout(() => {
            this.player.video.currentTime = res.data;
          }, 500);
        }
      }
    });
  }

  getListCommentOfCourse(idCourse: number) {
    this.courseServices.getListCourseEvaluation(idCourse).subscribe((res) => {
      if (res.retCode == 0 && res.systemMessage == "") {
        this.commentOfCourse = res.data;
        this.updatePlayerUrl(this.getVideoContentUrl());
      } else {
        this.messengerService.errorWithIssue();
      }
    });
  }

  likeForComment(id: number) {
    this.courseServices.likeForCommentCourse(id).subscribe((res) => {
      if (res.retCode == 0 && res.systemMessage == "") {
        this.getListCommentOfCourse(this.idCourse);
      } else {
        this.messengerService.errorWithIssue();
      }
    });
  }

  disLikeForComment(id: number) {
    this.courseServices.disLikeForCommentCourse(id).subscribe((res) => {
      if (res.retCode == 0 && res.systemMessage == "") {
        this.getListCommentOfCourse(this.idCourse);
      } else {
        this.messengerService.errorWithIssue();
      }
    });
  }

  getListTheadQuestionOfCourse(idCourse: number) {
    this.courseServices.getListTheadQuestionCourse(idCourse).subscribe((res) => {
      if (res.retCode == 0 && res.systemMessage == "") {
        this.theadQuestionOfCourse = res.data;
      } else {
        this.messengerService.errorWithIssue();
      }
    });
  }

  uploadSubtitle(language: string, label: string, fileInput: HTMLInputElement) {
    if (!language || !label || !fileInput.files || fileInput.files.length === 0) {
      this.messengerService.warringWithIssue();
      return;
    }
    const file = fileInput.files[0];
    const idContentDetail = this.videoOfContent?.id;
    if (!idContentDetail) return;

    this.subtitleService.addSubtitle(idContentDetail, language, label, file).subscribe(res => {
      if (res.statusCode == 200) {
        fileInput.value = '';
        this.getDataOfContentDetails(this.activeItemIndex!);
      } else {
        this.messengerService.errorWithIssue();
      }
    });
  }

  deleteSubtitle(id: number) {
    this.subtitleService.deleteSubtitle(id).subscribe(res => {
      if (res.statusCode == 200) {
        this.getDataOfContentDetails(this.activeItemIndex!);
      } else {
        this.messengerService.errorWithIssue();
      }
    });
  }

  getListUserNameRegisterForCourse(idCourse: number) {
    this.studentServices.getListUserNameRegisterForCourse(idCourse).subscribe((res) => {
      if (res.retCode == 0 && res.systemMessage == "") {
        this.listNameRegister = res.data;
      } else {
        this.messengerService.errorWithIssue();
      }
    });
  }

  private updatePlayerUrl(url?: string | null) {
    const youtubeVideoId = this.getYoutubeVideoId();
    if (youtubeVideoId) {
      this.youtubeEmbedUrl = this.sanitizer.bypassSecurityTrustResourceUrl(`https://www.youtube.com/embed/${youtubeVideoId}`);
      if (this.player) {
        this.player.pause();
      }
      return;
    }

    this.youtubeEmbedUrl = undefined;
    if (this.player && url) {
      this.player.url = url;
      if (this.subtitles && this.subtitles.length > 0) {
        this.player.subtitle.switch(this.subtitles[0].fileUrl, {
            name: this.subtitles[0].label
        });
      } else if (this.player.subtitle) {
        this.player.subtitle.show = false;
      }
    }
  }

  private getCourseVideoUrl(): string | undefined {
    if (!this.contentCourseDetail?.fileUrl) {
      return undefined;
    }

    return this.contentCourseDetail.fileUploadUrlStream ?? undefined;
  }

  private getVideoContentUrl(): string | undefined {
    if (!this.videoOfContent?.fileNameVideo) {
      return undefined;
    }

    return this.videoOfContent.fileUploadUrlStream ?? undefined;
  }

  private getYoutubeVideoId(): string | undefined {
    const lessonVideoId = this.videoOfContent?.idVideoUpload;
    const lessonFileName = this.videoOfContent?.fileNameVideo;
    if (lessonVideoId && lessonFileName === lessonVideoId && this.isYoutubeVideoId(lessonVideoId)) {
      return lessonVideoId;
    }

    const courseVideoId = this.contentCourseDetail?.fileUrl;
    const coursePlaylistId = this.contentCourseDetail?.keyVideoUpload;
    if (!this.videoOfContent && courseVideoId && coursePlaylistId && this.isYoutubeVideoId(courseVideoId)) {
      return courseVideoId;
    }

    return undefined;
  }

  private isYoutubeVideoId(value: string): boolean {
    return /^[a-zA-Z0-9_-]{11}$/.test(value);
  }

  // === Auto-Advance Countdown ===
  onVideoEnded() {
    this.showCountdown = true;
    this.countdownSeconds = 5;
    this.countdownTimer = setInterval(() => {
      this.countdownSeconds--;
      if (this.countdownSeconds <= 0) {
        this.cancelCountdown();
        this.goToNextLesson();
      }
    }, 1000);
  }

  cancelCountdown() {
    this.showCountdown = false;
    if (this.countdownTimer) {
      clearInterval(this.countdownTimer);
      this.countdownTimer = null;
    }
  }

  goToNextLesson() {
    if (this.contentCourseDetail?.listContentCourseDetails) {
      const details = this.contentCourseDetail.listContentCourseDetails;
      const currentIdx = details.findIndex(d => d.id === this.activeItemIndex);
      if (currentIdx >= 0 && currentIdx < details.length - 1) {
        const nextLesson = details[currentIdx + 1];
        this.selectContentCourse(nextLesson.id);
      }
    }
  }

  // === Auto-Save Draft ===
  private getDraftKey(): string {
    return `assignment_draft_${this.idCourse}_${this.selectedAssignment?.id ?? 'none'}`;
  }

  private startAutoSave() {
    this.autoSaveInterval = setInterval(() => {
      if (this.submissionText && this.selectedAssignment) {
        localStorage.setItem(this.getDraftKey(), this.submissionText);
      }
    }, 10000);
  }

  private restoreDraft() {
    // Will be called when assignment is selected, via viewAssignment override
  }

  private clearDraft() {
    localStorage.removeItem(this.getDraftKey());
  }

  ngOnDestroy() {
    document.removeEventListener('keydown', this.keyboardHandler);
    this.cancelCountdown();
    if (this.autoSaveInterval) {
      clearInterval(this.autoSaveInterval);
    }
    if (this.positionSaveInterval) {
      clearInterval(this.positionSaveInterval);
    }
    this.videoEngagementService.stopTracking();
    if (this.player) {
      this.player.destroy();
    }
  }

  calculateProgress() {
    if (!this.contentCourseDetail?.listContentCourseDetails) {
      this.courseProgress = 0;
      return;
    }
    let total = 0, completed = 0;
    this.contentCourseDetail.listContentCourseDetails.forEach((section: any) => {
      (section.contentAndContentDetails || []).forEach((lesson: any) => {
        total++;
        if (lesson.isCompleted) completed++;
      });
    });
    this.courseProgress = total > 0 ? Math.round((completed / total) * 100) : 0;
  }

  toggleLessonComplete(event: Event, lesson: any) {
    event.stopPropagation();
    const wasCompleted = lesson.isCompleted;
    lesson.isCompleted = !wasCompleted;
    this.calculateProgress();

    // Call API to persist the completion state
    if (!wasCompleted) {
      this.lessonCompletionService.markComplete(this.idCourse, lesson.id).subscribe({
        next: () => {
          if (this.courseProgress === 100) {
            this.confettiService.celebrate();
          }
        },
        error: () => {
          lesson.isCompleted = wasCompleted;
          this.calculateProgress();
        }
      });
    } else {
      this.lessonCompletionService.markIncomplete(this.idCourse, lesson.id).subscribe({
        error: () => {
          lesson.isCompleted = wasCompleted;
          this.calculateProgress();
        }
      });
    }

  }

  // === CERTIFICATE ===
  generateAndDownloadCertificate() {
    if (this.isCertLoading) return;
    this.isCertLoading = true;
    const userId = this.getUserId();

    this.certificateService.generate(this.idCourse, userId).subscribe({
      next: (res: any) => {
        const certId = res.data?.id;
        if (!certId) { this.isCertLoading = false; return; }
        this.certificateService.download(certId).subscribe({
          next: (blob: Blob) => {
            const url = URL.createObjectURL(blob);
            const a = document.createElement('a');
            a.href = url;
            a.download = `certificate-${this.idCourse}.pdf`;
            a.click();
            URL.revokeObjectURL(url);
            this.isCertLoading = false;
          },
          error: () => { this.isCertLoading = false; this.messengerService.errorWithIssue(); }
        });
      },
      error: () => { this.isCertLoading = false; this.messengerService.errorWithIssue(); }
    });
  }

  private getUserId(): number {
    const user = LocalStorageConfig.GetUser();
    return user?.userId ?? 0;
  }

  // === COURSE NOTES ===
  addCourseNote() {
    if (!this.newNoteContent.trim()) return;
    this.isAddingNote = true;
    const timestamp = this.player?.video?.currentTime ? Math.floor(this.player.video.currentTime) : this.newNoteTimestamp;
    this.courseNoteService.addNote({
      idCourse: this.idCourse,
      idContentDetail: this.activeItemIndex ?? 0,
      content: this.newNoteContent.trim(),
      timestamp
    }).subscribe({
      next: (res: any) => {
        if (res.retCode === 0) {
          this.notes.unshift(res.data);
          this.newNoteContent = '';
          this.messengerService.successes('Đã lưu ghi chú!');
        }
        this.isAddingNote = false;
      },
      error: () => { this.isAddingNote = false; this.messengerService.errorWithIssue(); }
    });
  }

  deleteNote(noteId: number) {
    this.courseNoteService.deleteNote(noteId).subscribe({
      next: () => {
        this.notes = this.notes.filter((n: any) => n.id !== noteId);
        this.messengerService.successes('Đã xoá ghi chú');
      },
      error: () => this.messengerService.errorWithIssue()
    });
  }

  formatNoteTimestamp(seconds: number): string {
    if (!seconds) return '0:00';
    const m = Math.floor(seconds / 60);
    const s = seconds % 60;
    return `${m}:${s.toString().padStart(2, '0')}`;
  }

  jumpToNoteTimestamp(seconds: number) {
    if (this.player?.video) {
      this.player.video.currentTime = seconds;
      this.player.play();
    }
  }

  // === LESSON NAVIGATION HELPERS ===
  private getAllLessons(): any[] {
    const lessons: any[] = [];
    (this.contentCourseDetail?.listContentCourseDetails || []).forEach((section: any) => {
      (section.contentAndContentDetails || []).forEach((l: any) => lessons.push(l));
    });
    return lessons;
  }

  getCurrentLessonIndex(): number {
    return this.getAllLessons().findIndex(l => l.id === this.activeItemIndex);
  }

  getTotalLessons(): number {
    return this.getAllLessons().length;
  }

  hasPrevLesson(): boolean {
    return this.getCurrentLessonIndex() > 0;
  }

  hasNextLesson(): boolean {
    const idx = this.getCurrentLessonIndex();
    return idx >= 0 && idx < this.getTotalLessons() - 1;
  }

  goToPrevLesson() {
    const lessons = this.getAllLessons();
    const idx = this.getCurrentLessonIndex();
    if (idx > 0) {
      this.selectContentCourse(lessons[idx - 1].id);
    }
  }

}
