import * as signalR from '@microsoft/signalr'
import { useAuthStore } from '../stores/auth.store'

const HUB_BASE_URL = (import.meta.env.VITE_API_BASE_URL || 'http://localhost:5267')
  .replace(/\/api\/?$/, '')

class SignalRService {
  private connection: signalR.HubConnection | null = null

  public initConnection() {
    if (!this.connection) {
      this.connection = new signalR.HubConnectionBuilder()
        .withUrl(`${HUB_BASE_URL}/attendanceHub`, {
          accessTokenFactory: () => useAuthStore().user?.token || ''
        })
        .withAutomaticReconnect()
        .build()
    }
    return this.connection
  }

  public async start() {
    if (this.connection && this.connection.state === signalR.HubConnectionState.Disconnected) {
      try {
        await this.connection.start()
        console.log("SignalR bağlantısı başarılı!")
      } catch (err) {
        console.error("SignalR bağlantı hatası: ", err)
      }
    }
  }

  public stop() {
    if (this.connection && this.connection.state === signalR.HubConnectionState.Connected) {
      this.connection.stop()
    }
  }

  public on(eventName: string, callback: (...args: any[]) => void) {
    if (this.connection) {
      this.connection.off(eventName) 
      this.connection.on(eventName, callback)
    }
  }
}

export default new SignalRService()