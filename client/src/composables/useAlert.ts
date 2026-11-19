import { useToast } from 'primevue/usetoast'
import { useConfirm } from 'primevue/useconfirm'

export const useAlert = () => {
  const toast = useToast()
  const confirm = useConfirm()

  const showSuccess = (detail: string, summary: string = 'Başarılı') => {
    toast.add({ severity: 'success', summary, detail, life: 3000 })
  }

  const showWarn = (detail: string, summary: string = 'Uyarı') => {
    toast.add({ severity: 'warn', summary, detail, life: 5000 })
  }

  const showError = (detail: string, summary: string = 'Hata') => {
    toast.add({ severity: 'error', summary, detail, life: 3000 })
  }

  const requireDangerConfirm = (message: string, acceptCallback: () => void) => {
    confirm.require({
      message: message,
      header: 'Silme Onayı',
      acceptClass: 'p-button-danger',
      acceptLabel: 'Evet, Sil',
      rejectLabel: 'İptal',
      accept: acceptCallback
    })
  }

  return {
    showSuccess,
    showWarn,
    showError,
    requireDangerConfirm
  }
}