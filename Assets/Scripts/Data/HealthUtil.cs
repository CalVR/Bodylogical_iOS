﻿public class HealthUtil {
    public static HealthStatus CalculateStatus(int point) {
        if (point < 30) {
            return HealthStatus.Bad;
        }

        if (point < 60) {
            return HealthStatus.Intermediate;
        }

        return HealthStatus.Good;
    }
}
