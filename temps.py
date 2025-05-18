import subprocess
import time

def get_cpu_temperature():
    try:
        with open('/sys/class/thermal/thermal_zone0/temp', 'r') as f:
            temp_raw = f.readline().strip()
            return float(temp_raw) / 1000.0
    except FileNotFoundError:
        try:
            output = subprocess.check_output(['/opt/vc/bin/vcgencmd', 'measure_temp']).decode('utf-8').strip()
            return float(output.split('=')[1][:-2])
        except FileNotFoundError:
            return None
    except Exception as e:
        print(f"Error reading temperature: {e}")
        return None

if __name__ == "__main__":
    while True:
        temperature = get_cpu_temperature()
        if temperature is not None:
            print(f"CPU Temperature: {temperature:.1f}°C", end='\r')
        else:
            print("Could not read temperature.", end='\r')
        time.sleep(1)