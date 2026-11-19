export const generateAttendanceQrPayload = (userId: number): string => {
  const payload = JSON.stringify({ id: userId, timestamp: Date.now() })
  return btoa(payload)
}