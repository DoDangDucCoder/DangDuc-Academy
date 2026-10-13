import { NativeStackScreenProps } from "@react-navigation/native-stack";
import { useState } from "react";
import { ActivityIndicator, Button, Pressable, StyleSheet, Text, TextInput, View } from "react-native";
import { AuthUser, login, register } from "../api/client";
import { RootStackParamList } from "../../App";

type Props = NativeStackScreenProps<RootStackParamList, "Login"> & {
  onSignedIn: (user: AuthUser) => void;
};

export function LoginScreen({ navigation, onSignedIn }: Props) {
  const [isRegister, setIsRegister] = useState(false);
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [name, setName] = useState("");
  const [phone, setPhone] = useState("");
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  async function submit() {
    setLoading(true);
    setError(null);

    try {
      const user = isRegister
        ? await register({ email, password, name, phone })
        : await login({ email, password });
      onSignedIn(user);
      navigation.replace("Courses");
    } catch (err) {
      setError(err instanceof Error ? err.message : "Unable to sign in");
    } finally {
      setLoading(false);
    }
  }

  return (
    <View style={styles.container}>
      {isRegister && <TextInput placeholder="Name" style={styles.input} value={name} onChangeText={setName} />}
      {isRegister && <TextInput placeholder="Phone" style={styles.input} value={phone} onChangeText={setPhone} keyboardType="phone-pad" />}
      <TextInput autoCapitalize="none" keyboardType="email-address" placeholder="Email" style={styles.input} value={email} onChangeText={setEmail} />
      <TextInput placeholder="Password" secureTextEntry style={styles.input} value={password} onChangeText={setPassword} />
      {!!error && <Text style={styles.error}>{error}</Text>}
      {loading ? <ActivityIndicator /> : <Button title={isRegister ? "Create account" : "Login"} onPress={submit} />}
      <Pressable onPress={() => setIsRegister((value) => !value)}>
        <Text style={styles.switchMode}>{isRegister ? "Already have an account? Login" : "Need an account? Register"}</Text>
      </Pressable>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    gap: 12,
    padding: 16
  },
  input: {
    borderColor: "#d6d6d6",
    borderRadius: 8,
    borderWidth: 1,
    padding: 12
  },
  error: {
    color: "#b00020"
  },
  switchMode: {
    color: "#2457c5",
    marginTop: 12,
    textAlign: "center"
  }
});
