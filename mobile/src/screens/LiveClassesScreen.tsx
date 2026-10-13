import { NativeStackScreenProps } from "@react-navigation/native-stack";
import { useEffect, useState } from "react";
import { ActivityIndicator, Button, Linking, ScrollView, StyleSheet, Text, View } from "react-native";
import { RootStackParamList } from "../../App";
import { LiveClass, getLiveClasses } from "../api/client";

type Props = NativeStackScreenProps<RootStackParamList, "LiveClasses">;

export function LiveClassesScreen({ route }: Props) {
  const [classes, setClasses] = useState<LiveClass[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    getLiveClasses(route.params.courseId)
      .then(setClasses)
      .catch((err: unknown) => setError(err instanceof Error ? err.message : "Unable to load live classes"))
      .finally(() => setLoading(false));
  }, [route.params.courseId]);

  if (loading) return <ActivityIndicator style={styles.centered} />;

  return (
    <ScrollView contentContainerStyle={styles.container}>
      {!!error && <Text style={styles.error}>{error}</Text>}
      {classes.length === 0 && <Text>No live classes scheduled yet.</Text>}
      {classes.map((item) => (
        <View key={item.id} style={styles.card}>
          <Text style={styles.title}>{item.title}</Text>
          {!!item.description && <Text style={styles.body}>{item.description}</Text>}
          <Text style={styles.body}>Start: {JSON.stringify(item.startTime)}</Text>
          <Text style={styles.body}>End: {JSON.stringify(item.endTime)}</Text>
          <Button title="Open meeting" onPress={() => Linking.openURL(item.meetingUrl)} />
        </View>
      ))}
    </ScrollView>
  );
}

const styles = StyleSheet.create({
  centered: { flex: 1 },
  container: { gap: 12, padding: 16 },
  card: { borderColor: "#d6d6d6", borderRadius: 8, borderWidth: 1, padding: 16 },
  title: { fontSize: 18, fontWeight: "700" },
  body: { color: "#555", marginVertical: 4 },
  error: { color: "#b00020" }
});
