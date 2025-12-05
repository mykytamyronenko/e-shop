export interface UserMembership {
  userMembershipId: number;
  userId: number;
  membershipId: number;
  startDate: string;
  endDate: string;
  status: 'active' | 'expired' | 'canceled';
}
