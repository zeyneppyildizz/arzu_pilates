import api from './api';
import type { MeasurementDto } from '../types/measurement.type'

const controller = '/users';

export const measurementService = {
  async getMeasurements(studentId: number): Promise<MeasurementDto> {
    const response = await api.get(`${controller}/${studentId}/measurements`)
    return response.data
  },

  async saveMeasurements(studentId: number, measurements: MeasurementDto) {
    const response = await api.post(`${controller}/${studentId}/measurements`, measurements)
    return response.data
  }
}
