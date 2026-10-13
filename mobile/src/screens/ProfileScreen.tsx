import { useEffect, useState } from "react";
import { ActivityIndicator, Button, StyleSheet, Text, View } from "react-native";
import { AuthUser, getProfile } from "../api/client";

type Props = {
  user: AuthUser | null;
  onLogout: () => Promise<void>;
};

export function ProfileScreen({ user, onLogout }: Props) {
  const [profile, setProfile] = useState<AuthUser | null>(user);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(!!user?.userId);

  useEffect(() => {
    if (!user?.userId) {
      setLoading(false);
      return;
    }

    getProfile(user.userId)
      .then(setProfile)
      .catch((err: unknown) => setError(err instanceof Error ? err.message : "Unable to load profile"))
      .finally(() => setLoading(false));
  }, [user?.userId]);

  if (!user) {
    return <Text style={styles.message}>You are not signed in.</Text>;
  }

  if (loading) {
    return <ActivityIndicator style={styles.centered} />;
  }

  return (
    <View style={styles.container}>
      {!!error && <Text style={styles.error}>{error}</Text>}
      <Text style={styles.name}>{profile?.name ?? user.name ?? "Student"}</Text>
      <Text style={styles.body}>{profile?.email ?? user.email}</Text>
      <Text style={styles.body}>{profile?.phone ?? user.phone}</Text>
      <Text style={styles.body}>{profile?.roleName ?? user.roleName}</Text>
      <Button title="Logout" onPress={onLogout} />
    </View>
  );
}

const styles = StyleSheet.create({
  centered: {
    flex: 1
  },
  container: {
    flex: 1,
    gap: 10,
    padding: 16
  },
  name: {
    fontSize: 22,
    fontWeight: "700"
  },
  body: {
    color: "#555"
  },
  error: {
    color: "#b00020"
  },
  message: {
    padding: 16
  }
});
