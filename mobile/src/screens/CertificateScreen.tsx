import { NativeStackScreenProps } from "@react-navigation/native-stack";
import { useEffect, useState } from "react";
import { ActivityIndicator, Button, Linking, StyleSheet, Text, View } from "react-native";
import { RootStackParamList } from "../../App";
import { Certificate, generateCertificate, getCertificate, getCertificateDownloadUrl, getStoredUser } from "../api/client";

type Props = NativeStackScreenProps<RootStackParamList, "Certificate">;

export function CertificateScreen({ route }: Props) {
  const [certificate, setCertificate] = useState<Certificate | null>(null);
  const [userId, setUserId] = useState(0);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    getStoredUser()
      .then(async (user) => {
        const id = user?.userId ?? 0;
        setUserId(id);
        if (id > 0) {
          setCertificate(await getCertificate(route.params.courseId, id));
        }
      })
      .catch(() => setCertificate(null))
      .finally(() => setLoading(false));
  }, [route.params.courseId]);

  async function createCertificate() {
    setError(null);
    try {
      setCertificate(await generateCertificate(route.params.courseId, userId));
    } catch (err) {
      setError(err instanceof Error ? err.message : "Course is not completed yet");
    }
  }

  if (loading) return <ActivityIndicator style={styles.centered} />;
  if (!userId) return <Text style={styles.message}>Login to manage certificates.</Text>;

  return (
    <View style={styles.container}>
      {!!error && <Text style={styles.error}>{error}</Text>}
      {certificate ? (
        <>
          <Text style={styles.title}>Certificate ready</Text>
          <Text style={styles.body}>Code: {certificate.certificateCode}</Text>
          <Button title="Download certificate" onPress={() => Linking.openURL(getCertificateDownloadUrl(certificate.id))} />
        </>
      ) : (
        <>
          <Text style={styles.body}>Generate a certificate after completing all active lessons.</Text>
          <Button title="Generate certificate" onPress={createCertificate} />
        </>
      )}
    </View>
  );
}

const styles = StyleSheet.create({
  centered: { flex: 1 },
  container: { flex: 1, gap: 10, padding: 16 },
  title: { fontSize: 20, fontWeight: "700" },
  body: { color: "#555" },
  error: { color: "#b00020" },
  message: { padding: 16 }
});
