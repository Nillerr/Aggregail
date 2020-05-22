import {RecordedEvent} from "../components/StreamHub";

export interface User {
  id: string;
  username: string;
  fullName: string;
}

export interface RecentStream {
  name: string;
}

export interface RecentStreams {
  recentlyCreatedStreams: RecentStream[];
  recentlyChangedStreams: RecentStream[];
}

export interface StreamResponse {
  events: RecordedEvent[];
}

export interface CreateUserData {
  username: string;
  fullName: string;
  password: string;
  confirmedPassword: string;
}