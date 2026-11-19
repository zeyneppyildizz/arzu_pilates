const currencyFormatter = new Intl.NumberFormat('tr-TR', {
  style: 'currency',
  currency: 'TRY',
  maximumFractionDigits: 2
})

export const formatCurrency = (amount: number): string => currencyFormatter.format(amount)