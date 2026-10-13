import { NativeStackScreenProps } from "@react-navigation/native-stack";
import { useEffect, useState } from "react";
import { ActivityIndicator, Button, ScrollView, StyleSheet, Text, View } from "react-native";
import { CourseSummary, getCourseDetail } from "../api/client";
import { RootStackParamList } from "../../App";

type Props = NativeStackScreenProps<RootStackParamList, "CourseDetail">;

export function CourseDetailScreen({ navigation, route }: Props) {
  const [course, setCourse] = useState<CourseSummary | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    getCourseDetail(route.params.courseId)
      .then(setCourse)
      .catch((err: unknown) => setError(err instanceof Error ? err.message : "Unable to load course"))
      .finally(() => setLoading(false));
  }, [route.params.courseId]);

  if (loading) {
    return <ActivityIndicator style={styles.centered} />;
  }

  if (error) {
    return <Text style={styles.message}>{error}</Text>;
  }

  return (
    <ScrollView contentContainerStyle={styles.container}>
      <Text style={styles.title}>{course?.title ?? route.params.title}</Text>
      {!!course?.shortDescription && <Text style={styles.body}>{course.shortDescription}</Text>}
      {!!course?.description && <Text style={styles.body}>{course.description}</Text>}
      <View style={styles.actions}>
        <Button title="Quiz" onPress={() => navigation.navigate("Quiz", { courseId: route.params.courseId, title: route.params.title })} />
        <Button title="Live classes" onPress={() => navigation.navigate("LiveClasses", { courseId: route.params.courseId, title: route.params.title })} />
        <Button title="Certificate" onPress={() => navigation.navigate("Certificate", { courseId: route.params.courseId, title: route.params.title })} />
      </View>
      <Text style={styles.sectionTitle}>Content</Text>
      {(course?.listContentCourseDetails ?? []).map((section) => (
        <View key={section.id} style={styles.section}>
          <Text style={styles.sectionName}>{section.title ?? section.contentCourseName ?? "Section"}</Text>
          {(section.listContentCourseDetails ?? []).map((item) => (
            <Text key={item.id} style={styles.lesson}>{item.title ?? item.detailCourseName ?? "Lesson"}</Text>
          ))}
        </View>
      ))}
      {(course?.listContentCourseDetails?.length ?? 0) === 0 && <Text style={styles.body}>No lessons published yet.</Text>}
    </ScrollView>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    padding: 16
  },
  title: {
    fontSize: 22,
    fontWeight: "700"
  },
  body: {
    color: "#555",
    marginTop: 8
  },
  centered: {
    flex: 1
  },
  message: {
    padding: 16
  },
  sectionTitle: {
    fontSize: 18,
    fontWeight: "700",
    marginTop: 24
  },
  section: {
    borderColor: "#d6d6d6",
    borderRadius: 8,
    borderWidth: 1,
    marginTop: 12,
    padding: 12
  },
  sectionName: {
    fontWeight: "700"
  },
  lesson: {
    color: "#555",
    marginTop: 8
  },
  actions: {
    gap: 8,
    marginTop: 16
  }
});
