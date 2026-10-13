import { NativeStackScreenProps } from "@react-navigation/native-stack";
import { useEffect, useState } from "react";
import { ActivityIndicator, FlatList, Pressable, StyleSheet, Text, TextInput, View } from "react-native";
import { CourseSummary, getCourses } from "../api/client";
import { RootStackParamList } from "../../App";

type Props = NativeStackScreenProps<RootStackParamList, "Courses">;

export function CourseListScreen({ navigation }: Props) {
  const [courses, setCourses] = useState<CourseSummary[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [keyword, setKeyword] = useState("");
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const handle = setTimeout(() => {
      setLoading(true);
      setError(null);
      getCourses(keyword)
      .then(setCourses)
      .catch((err: unknown) => setError(err instanceof Error ? err.message : "Unable to load courses"))
      .finally(() => setLoading(false));
    }, 300);

    return () => clearTimeout(handle);
  }, [keyword]);

  return (
    <View style={styles.screen}>
      <TextInput
        autoCapitalize="none"
        placeholder="Search courses"
        style={styles.search}
        value={keyword}
        onChangeText={setKeyword}
      />
      {loading && <ActivityIndicator style={styles.centered} />}
      {!!error && <Text style={styles.message}>{error}</Text>}
      {!loading && !error && (
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
            </Pressable>
          )}
          ListEmptyComponent={<Text style={styles.message}>No courses found.</Text>}
        />
      )}
    </View>
  );
}

const styles = StyleSheet.create({
  screen: {
    flex: 1
  },
  centered: {
    flex: 1
  },
  search: {
    borderColor: "#d6d6d6",
    borderRadius: 8,
    borderWidth: 1,
    margin: 16,
    marginBottom: 0,
    padding: 12
  },
  list: {
    padding: 16,
    gap: 12
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
  message: {
    padding: 16
  }
});
