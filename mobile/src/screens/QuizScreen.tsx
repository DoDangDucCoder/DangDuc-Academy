import { NativeStackScreenProps } from "@react-navigation/native-stack";
import { useEffect, useState } from "react";
import { ActivityIndicator, Button, Pressable, ScrollView, StyleSheet, Text, View } from "react-native";
import { RootStackParamList } from "../../App";
import { Quiz, QuizResult, getCourseQuizzes, submitQuiz } from "../api/client";

type Props = NativeStackScreenProps<RootStackParamList, "Quiz">;

export function QuizScreen({ route }: Props) {
  const [quizzes, setQuizzes] = useState<Quiz[]>([]);
  const [answers, setAnswers] = useState<Record<number, number>>({});
  const [result, setResult] = useState<QuizResult | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    getCourseQuizzes(route.params.courseId)
      .then(setQuizzes)
      .catch((err: unknown) => setError(err instanceof Error ? err.message : "Unable to load quizzes"))
      .finally(() => setLoading(false));
  }, [route.params.courseId]);

  async function submit(quiz: Quiz) {
    setError(null);
    if (quiz.questions.some((question) => !answers[question.id])) {
      setError("Answer every question before submitting.");
      return;
    }

    const payload = quiz.questions
      .map((question) => ({ idQuestion: question.id, idAnswerOption: answers[question.id] }));

    try {
      setResult(await submitQuiz(quiz.id, payload));
    } catch (err) {
      setError(err instanceof Error ? err.message : "Unable to submit quiz");
    }
  }

  if (loading) return <ActivityIndicator style={styles.centered} />;

  return (
    <ScrollView contentContainerStyle={styles.container}>
      {!!error && <Text style={styles.error}>{error}</Text>}
      {quizzes.length === 0 && <Text>No quiz published yet.</Text>}
      {quizzes.map((quiz) => (
        <View key={quiz.id} style={styles.card}>
          <Text style={styles.title}>{quiz.title}</Text>
          {!!quiz.description && <Text style={styles.body}>{quiz.description}</Text>}
          <Text style={styles.body}>Passing score: {quiz.passingScore}%</Text>
          {quiz.questions.map((question) => (
            <View key={question.id} style={styles.question}>
              <Text style={styles.questionText}>{question.questionText}</Text>
              {question.answers.map((answer) => (
                <Pressable key={answer.id} style={styles.answer} onPress={() => setAnswers((current) => ({ ...current, [question.id]: answer.id }))}>
                  <Text>{answers[question.id] === answer.id ? "●" : "○"} {answer.answerText}</Text>
                </Pressable>
              ))}
            </View>
          ))}
          <Button title="Submit quiz" onPress={() => submit(quiz)} />
        </View>
      ))}
      {result && <Text style={result.isPassed ? styles.pass : styles.fail}>Score {result.score}/{result.totalScore}: {result.isPassed ? "Passed" : "Not passed"}</Text>}
    </ScrollView>
  );
}

const styles = StyleSheet.create({
  centered: { flex: 1 },
  container: { gap: 12, padding: 16 },
  card: { borderColor: "#d6d6d6", borderRadius: 8, borderWidth: 1, padding: 16 },
  title: { fontSize: 18, fontWeight: "700" },
  body: { color: "#555", marginTop: 6 },
  question: { marginTop: 14 },
  questionText: { fontWeight: "700" },
  answer: { paddingVertical: 8 },
  error: { color: "#b00020" },
  pass: { color: "#067a46", fontWeight: "700" },
  fail: { color: "#b26a00", fontWeight: "700" }
});
