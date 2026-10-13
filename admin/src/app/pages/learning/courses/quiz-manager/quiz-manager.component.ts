import { Component, OnInit } from '@angular/core';
import { CourseServices } from 'src/app/core/services/course.service';
import { Phase4AdminService } from 'src/app/core/services/phase4-admin.service';
import { Course } from 'src/app/models/models/course';
import { QuizModel } from 'src/app/models/models/phase4-admin.models';

@Component({
  selector: 'app-quiz-manager',
  templateUrl: './quiz-manager.component.html',
  styleUrls: ['./quiz-manager.component.scss']
})
export class QuizManagerComponent implements OnInit {
  idCourse = 0;
  courses: Course[] = [];
  quizzes: QuizModel[] = [];
  error = '';
  success = '';
  isLoading = false;
  isLoadingCourses = false;
  model: QuizModel = this.createEmptyQuiz();

  constructor(
    private readonly phase4AdminService: Phase4AdminService,
    private readonly courseServices: CourseServices
  ) {}

  ngOnInit(): void {
    this.loadCourses();
  }

  loadCourses() {
    this.isLoadingCourses = true;
    this.courseServices.getListAllCourse().subscribe({
      next: (res) => {
        this.courses = res.data ?? [];
        this.isLoadingCourses = false;
      },
      error: () => {
        this.courses = [];
        this.isLoadingCourses = false;
      }
    });
  }

  load() {
    if (!this.idCourse) {
      this.error = 'Course id is required.';
      return;
    }

    this.isLoading = true;
    this.error = '';
    this.success = '';
    this.phase4AdminService.getQuizzesByCourse(this.idCourse).subscribe({
      next: (res) => {
        this.quizzes = res.data ?? [];
        this.model.idCourse = this.idCourse;
        this.isLoading = false;
      },
      error: () => {
        this.error = 'Unable to load quizzes.';
        this.isLoading = false;
      }
    });
  }

  addQuestion() {
    this.model.questions.push({
      questionText: '',
      point: 1,
      answers: [
        { answerText: '', isCorrect: true },
        { answerText: '', isCorrect: false }
      ]
    });
  }

  addAnswer(questionIndex: number) {
    this.model.questions[questionIndex].answers.push({ answerText: '', isCorrect: false });
  }

  removeQuestion(questionIndex: number) {
    if (this.model.questions.length <= 1) {
      this.error = 'Quiz needs at least one question.';
      return;
    }
    this.model.questions.splice(questionIndex, 1);
  }

  removeAnswer(questionIndex: number, answerIndex: number) {
    const answers = this.model.questions[questionIndex].answers;
    if (answers.length <= 2) {
      this.error = 'Each question needs at least two answers.';
      return;
    }
    answers.splice(answerIndex, 1);
  }

  edit(quiz: QuizModel) {
    this.model = JSON.parse(JSON.stringify(quiz));
    this.idCourse = quiz.idCourse;
  }

  reset() {
    this.model = this.createEmptyQuiz();
    this.model.idCourse = this.idCourse;
  }

  save() {
    const validation = this.validate();
    if (validation) {
      this.error = validation;
      this.success = '';
      return;
    }

    this.model.idCourse = this.idCourse || this.model.idCourse;
    const onSuccess = () => {
      this.success = 'Quiz saved successfully.';
      this.reset();
      this.load();
    };
    const onError = () => {
      this.error = 'Unable to save quiz. Check title, passing score, questions, and correct answers.';
    };

    if (this.model.id) {
      this.phase4AdminService.updateQuiz(this.model.id, this.model).subscribe({
        next: onSuccess,
        error: onError
      });
      return;
    }

    this.phase4AdminService.createQuiz(this.model).subscribe({
      next: () => {
        this.reset();
        this.load();
      },
      error: onError
    });
  }

  private createEmptyQuiz(): QuizModel {
    return {
      idCourse: 0,
      title: '',
      description: '',
      passingScore: 70,
      questions: [
        {
          questionText: '',
          point: 1,
          answers: [
            { answerText: '', isCorrect: true },
            { answerText: '', isCorrect: false }
          ]
        }
      ]
    };
  }

  private validate(): string {
    if (!this.idCourse && !this.model.idCourse) return 'Course is required.';
    if (!this.model.title?.trim()) return 'Quiz title is required.';
    if (this.model.passingScore < 0 || this.model.passingScore > 100) return 'Passing score must be between 0 and 100.';
    if (!this.model.questions.length) return 'Quiz needs at least one question.';

    for (const question of this.model.questions) {
      if (!question.questionText?.trim()) return 'Every question needs text.';
      if (question.point <= 0) return 'Question point must be greater than 0.';
      if (question.answers.length < 2) return 'Every question needs at least two answers.';
      if (!question.answers.some(answer => answer.isCorrect)) return 'Every question needs one correct answer.';
      if (question.answers.some(answer => !answer.answerText?.trim())) return 'Every answer needs text.';
    }

    return '';
  }
}
