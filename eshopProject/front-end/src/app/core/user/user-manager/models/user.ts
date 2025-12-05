export interface User {
  userId: number;
  username: string;
  email: string;
  password: string;
  profilePicture: string;
  membershipLevel: string;
  balance: number;
}

export interface AdminUser {
  userId: number;
  username: string;
  email: string;
  password: string;
  role: 'connected_user' | 'admin';
  profilePicture: string;
  membershipLevel: string;
  rating: number;
  status: 'active' | 'suspended' | 'deleted';
  balance: number;
}

export interface FindUser {
  userId: number;
  username: string;
  role: 'connected_user' | 'admin';
  profilePicture: string;
  membershipLevel: string;
  rating: number;
  status: 'active' | 'suspended' | 'deleted';
}
