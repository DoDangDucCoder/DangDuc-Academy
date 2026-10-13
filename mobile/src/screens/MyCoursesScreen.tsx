import { NativeStackScreenProps } from "@react-navigation/native-stack";
import { useEffect, useState } from "react";
import { ActivityIndicator, FlatList, Pressable, StyleSheet, Text, View } from "react-native";
import { RootStackParamList } from "../../App";
import { AuthUser, CourseSummary, getMyCourses } from "../api/client";

type Props = NativeStackScreenProps<RootStackParamList, "MyCourses"> & {
  user: AuthUser | null;
};

export function MyCoursesScreen({ navigation, user }: Props) {
  const [courses, setCourses] = useState<CourseSummary[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (!user?.userId) {
      setLoading(false);
      return;
    }

    getMyCourses(user.userId)
      .then(setCourses)
      .catch((err: unknown) => setError(err instanceof Error ? err.message : "Unable to load your courses"))
      .finally(() => setLoading(false));
  }, [user?.userId]);

  if (!user) {
    return <Text style={styles.message}>Login to see your courses.</Text>;
  }

  if (loading) {
    return <ActivityIndicator style={styles.centered} />;
  }

  if (error) {
    return <Text style={styles.message}>{error}</Text>;
  }

  return (
    <FlatList
      contentContainerStyle={styles.list}
      data={courses}
      keyExtractor={(item) => String(item.id)}
      renderItem={({ item }) => (
        <Pressable
          style={styles.course}
          onPress={() => navigation.navigate("CourseDetail", { courseId: item.id, title: item.title })}
        >
          <Text style={styles.title}>{item.title}</Text>
          {!!item.shortDescription && <Text style={styles.description}>{item.shortDescription}</Text>}
          <Text style={styles.link}>Continue learning</Text>
        </Pressable>
      )}
      ListEmptyComponent={<Text style={styles.message}>No enrolled courses yet.</Text>}
    />
  );
}

const styles = StyleSheet.create({
  centered: {
    flex: 1
  },
  list: {
    gap: 12,
    padding: 16
  },
  course: {
    borderColor: "#d6d6d6",
    borderRadius: 8,
    borderWidth: 1,
    padding: 16
  },
  title: {
    fontSize: 17,
    fontWeight: "700"
  },
  description: {
    color: "#555",
    marginTop: 6
  },
  link: {
    color: "#2563eb",
    fontWeight: "700",
    marginTop: 10
  },
  message: {
    padding: 16
  }
});
