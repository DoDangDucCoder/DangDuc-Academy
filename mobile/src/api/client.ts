import AsyncStorage from "@react-native-async-storage/async-storage";

const ACCESS_TOKEN_KEY = "hdnx.accessToken";
const REFRESH_TOKEN_KEY = "hdnx.refreshToken";
const USER_KEY = "hdnx.user";

export type AuthUser = {
  userId: number;
  name?: string;
  email?: string;
  phone?: string;
  roleName?: string;
  token?: string;
  refreshToken?: string;
};

export type CourseSummary = {
  id: number;
  title: string;
  shortDescription?: string;
  description?: string;
  imageUrl?: string;
  isPurchase?: boolean;
  priceOfCourse?: number;
  listContentCourseDetails?: CourseContentSection[];
};

export type CourseContentSection = {
  id: number;
  title?: string;
  contentCourseName?: string;
  listContentCourseDetails?: CourseContentDetail[];
};

export type CourseContentDetail = {
  id: number;
  title?: string;
  detailCourseName?: string;
  urlVideo?: string;
  timeVideo?: string;
};

export type Quiz = {
  id: number;
  idCourse: number;
  title: string;
  description?: string;
  passingScore: number;
  questions: QuizQuestion[];
};

export type QuizQuestion = {
  id: number;
  questionText: string;
  answers: QuizAnswer[];
};

export type QuizAnswer = {
  id: number;
  answerText: string;
};

export type QuizResult = {
  attemptId: number;
  score: number;
  totalScore: number;
  passingScore: number;
  isPassed: boolean;
};

export type LiveClass = {
  id: number;
  title: string;
  description?: string;
  startTime: string;
  endTime: string;
  meetingUrl: string;
};

export type Certificate = {
  id: number;
  certificateCode: string;
  issuedDate: string;
};

export type LoginRequest = {
  email: string;
  password: string;
};

export type RegisterRequest = LoginRequest & {
  name: string;
  phone: string;
};

type RequestOptions = RequestInit & {
  auth?: boolean;
  skipRefresh?: boolean;
};

function getApiBaseUrl() {
  const value = process.env.EXPO_PUBLIC_API_BASE_URL?.replace(/\/$/, "");
  if (!value) {
    throw new Error("Missing EXPO_PUBLIC_API_BASE_URL.");
  }

  return value;
}

function unwrap<T>(payload: unknown): T {
  if (payload && typeof payload === "object" && "data" in payload) {
    return (payload as { data: T }).data;
  }

  return payload as T;
}

async function request<T>(path: string, options: RequestOptions = {}): Promise<T> {
  const headers = new Headers(options.headers);
  headers.set("Accept", "application/json");

  if (options.body && !headers.has("Content-Type")) {
    headers.set("Content-Type", "application/json");
  }

  if (options.auth !== false) {
    const token = await AsyncStorage.getItem(ACCESS_TOKEN_KEY);
    if (token) {
      headers.set("Authorization", `Bearer ${token}`);
    }
  }

  const response = await fetch(`${getApiBaseUrl()}${path}`, {
    ...options,
    headers
  });

  if (response.status === 401 && options.auth !== false && !options.skipRefresh) {
    const refreshed = await refreshToken();
    if (refreshed?.token) {
      return request<T>(path, { ...options, skipRefresh: true });
    }
  }

  const text = await response.text();
  const payload = text ? JSON.parse(text) : null;

  if (!response.ok) {
    const message = payload?.systemMessage ?? payload?.message ?? `Request failed: ${response.status}`;
    throw new Error(message);
  }

  return unwrap<T>(payload);
}

async function saveSession(user: AuthUser) {
  if (user.token) {
    await AsyncStorage.setItem(ACCESS_TOKEN_KEY, user.token);
  }

  if (user.refreshToken) {
    await AsyncStorage.setItem(REFRESH_TOKEN_KEY, user.refreshToken);
  }

  await AsyncStorage.setItem(USER_KEY, JSON.stringify(user));
}

export async function getStoredUser(): Promise<AuthUser | null> {
  const raw = await AsyncStorage.getItem(USER_KEY);
  return raw ? JSON.parse(raw) : null;
}

export async function login(model: LoginRequest): Promise<AuthUser> {
  const user = await request<AuthUser>("/api/v1/authentication/login", {
    method: "POST",
    auth: false,
    body: JSON.stringify(model)
  });
  await saveSession(user);
  return user;
}

export async function register(model: RegisterRequest): Promise<AuthUser> {
  const user = await request<AuthUser>("/api/v1/authentication/register", {
    method: "POST",
    auth: false,
    body: JSON.stringify(model)
  });
  await saveSession(user);
  return user;
}

export async function refreshToken(): Promise<AuthUser | null> {
  const refreshTokenValue = await AsyncStorage.getItem(REFRESH_TOKEN_KEY);
  if (!refreshTokenValue) {
    return null;
  }

  const user = await request<AuthUser>("/api/v1/authentication/refresh-token", {
    method: "POST",
    auth: false,
    body: JSON.stringify({ refreshToken: refreshTokenValue })
  });
  await saveSession(user);
  return user;
}

export async function logout() {
  const refreshTokenValue = await AsyncStorage.getItem(REFRESH_TOKEN_KEY);
  await request("/api/v1/authentication/revoke-token", {
    method: "POST",
    body: JSON.stringify({ refreshToken: refreshTokenValue })
  }).catch(() => undefined);
  await AsyncStorage.multiRemove([ACCESS_TOKEN_KEY, REFRESH_TOKEN_KEY, USER_KEY]);
}

export async function getCourses(keyword = ""): Promise<CourseSummary[]> {
  const query = keyword ? `?Keyword=${encodeURIComponent(keyword)}` : "";
  const result = await request<{ results?: CourseSummary[] } | CourseSummary[]>(
    `/api/v1/master-course/course/search/1/20${query}`,
    { auth: false }
  );
  return Array.isArray(result) ? result : result.results ?? [];
}

export async function getCourseDetail(courseId: number): Promise<CourseSummary> {
  return request<CourseSummary>(`/api/v1/master-course/course/${courseId}/false`);
}

export async function getMyCourses(userId: number): Promise<CourseSummary[]> {
  return request<CourseSummary[]>(`/api/v1/student/student-course/${userId}`);
}

export async function getProfile(userId: number): Promise<AuthUser> {
  return request<AuthUser>(`/api/v1/student/${userId}`);
}

export async function getCourseQuizzes(courseId: number): Promise<Quiz[]> {
  return request<Quiz[]>(`/api/v1/quiz/course/${courseId}`, { auth: false });
}

export async function submitQuiz(idQuiz: number, answers: Array<{ idQuestion: number; idAnswerOption: number }>): Promise<QuizResult> {
  return request<QuizResult>("/api/v1/quiz/submit", {
    method: "POST",
    body: JSON.stringify({ idQuiz, answers })
  });
}

export async function getLiveClasses(courseId: number): Promise<LiveClass[]> {
  return request<LiveClass[]>(`/api/v1/live-class?idCourse=${courseId}`, { auth: false });
}

export async function getCertificate(courseId: number, userId: number): Promise<Certificate | null> {
  return request<Certificate | null>(`/api/v1/certificate/course/${courseId}/student/${userId}`);
}

export async function generateCertificate(courseId: number, userId: number): Promise<Certificate> {
  return request<Certificate>(`/api/v1/certificate/course/${courseId}/student/${userId}`, {
    method: "POST",
    body: JSON.stringify({})
  });
}

export function getCertificateDownloadUrl(certificateId: number) {
  return `${getApiBaseUrl()}/api/v1/certificate/${certificateId}/download`;
}
