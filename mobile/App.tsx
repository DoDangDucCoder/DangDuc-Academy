import { NavigationContainer } from "@react-navigation/native";
import { createNativeStackNavigator } from "@react-navigation/native-stack";
import { StatusBar } from "expo-status-bar";
import { useEffect, useState } from "react";
import { ActivityIndicator, Button, View } from "react-native";
import { AuthUser, getStoredUser, logout } from "./src/api/client";
import { CourseListScreen } from "./src/screens/CourseListScreen";
import { CourseDetailScreen } from "./src/screens/CourseDetailScreen";
import { LoginScreen } from "./src/screens/LoginScreen";
import { MyCoursesScreen } from "./src/screens/MyCoursesScreen";
import { ProfileScreen } from "./src/screens/ProfileScreen";
import { QuizScreen } from "./src/screens/QuizScreen";
import { LiveClassesScreen } from "./src/screens/LiveClassesScreen";
import { CertificateScreen } from "./src/screens/CertificateScreen";

export type RootStackParamList = {
  Login: undefined;
  Courses: undefined;
  CourseDetail: { courseId: number; title: string };
  MyCourses: undefined;
  Profile: undefined;
  Quiz: { courseId: number; title: string };
  LiveClasses: { courseId: number; title: string };
  Certificate: { courseId: number; title: string };
};

const Stack = createNativeStackNavigator<RootStackParamList>();

export default function App() {
  const [user, setUser] = useState<AuthUser | null>(null);
  const [booting, setBooting] = useState(true);

  useEffect(() => {
    getStoredUser()
      .then(setUser)
      .finally(() => setBooting(false));
  }, []);

  if (booting) {
    return (
      <View style={{ flex: 1, justifyContent: "center" }}>
        <ActivityIndicator />
      </View>
    );
  }

  return (
    <NavigationContainer>
      <StatusBar style="auto" />
      <Stack.Navigator>
        {!user && <Stack.Screen name="Login" options={{ title: "Sign in" }}>{(props) => <LoginScreen {...props} onSignedIn={setUser} />}</Stack.Screen>}
        <Stack.Screen
          name="Courses"
          component={CourseListScreen}
          options={({ navigation }) => ({
            title: "Courses",
            headerRight: () => <Button title={user ? "Me" : "Login"} onPress={() => navigation.navigate(user ? "Profile" : "Login")} />
          })}
        />
        <Stack.Screen name="CourseDetail" component={CourseDetailScreen} options={({ route }) => ({ title: route.params.title })} />
        <Stack.Screen name="Quiz" component={QuizScreen} options={({ route }) => ({ title: `${route.params.title} quiz` })} />
        <Stack.Screen name="LiveClasses" component={LiveClassesScreen} options={({ route }) => ({ title: `${route.params.title} live` })} />
        <Stack.Screen name="Certificate" component={CertificateScreen} options={({ route }) => ({ title: `${route.params.title} certificate` })} />
        <Stack.Screen name="MyCourses" options={{ title: "My courses" }}>{(props) => <MyCoursesScreen {...props} user={user} />}</Stack.Screen>
        <Stack.Screen name="Profile" options={{ title: "Profile" }}>
          {() => <ProfileScreen user={user} onLogout={async () => { await logout(); setUser(null); }} />}
        </Stack.Screen>
      </Stack.Navigator>
    </NavigationContainer>
  );
}
