import csv
import pyodbc
from datetime import datetime

# Cadena de conexión a la base de datos
connection_string = ("DRIVER={ODBC Driver 17 for SQL Server};"
                     "SERVER=103.195.101.157;"
                     "DATABASE=CORE_ALFIN;"
                     "UID=programacion;"
                     "PWD=2345;"
                     "TrustServerCertificate=yes;")

# Función para leer el CSV y actualizar la tabla SQL
def actualizar_datos_csv(csv_file):
    try:
        # Conectar a la base de datos
        conn = pyodbc.connect(connection_string)
        cursor = conn.cursor()
        cursor.fast_executemany = True

        # Leer el archivo CSV
        with open(csv_file, mode='r', encoding='ISO-8859-1') as file:
            reader = csv.DictReader(file)

            # Función para convertir valores vacíos en NULL
            def convertir_a_null(valor):
                return None if valor.strip() == "" else valor

            # Lista para almacenar los datos a actualizar
            datos_a_procesar = []
            contador = 0

            for row in reader:
                datos_a_procesar.append((
                    convertir_a_null(row['tasa_1']),
                    convertir_a_null(row['tasa_2']),
                    convertir_a_null(row['tasa_3']),
                    convertir_a_null(row['tasa_4']),
                    convertir_a_null(row['tasa_5']),
                    convertir_a_null(row['tasa_6']),
                    convertir_a_null(row['tasa_7']),
                    convertir_a_null(row['oferta_max']),
                    convertir_a_null(row['plazo']), 
                    convertir_a_null(row['CAPACIDAD_PAGO_MEN']),
                    convertir_a_null(row['campana_grupo']),
                    convertir_a_null(row['color_id']),
                    convertir_a_null(row['usuario']),
                    convertir_a_null(row['RANGO_DEUDA']),
                    convertir_a_null(row['NumEntidades']),
                    convertir_a_null(row['fresco']),
                    datetime(2025, 2, 2, 0, 0, 0),  # Aquí agregamos la fecha fija
                    convertir_a_null(row['dni'])  # DNI (clave para actualizar)
                ))
                
                # Ejecutar en lotes de 1000 registros
                if len(datos_a_procesar) >= 1000:
                    cursor.executemany("""
                        MERGE INTO [CORE_ALFIN].[dbo].[base_clientes_banco] AS target
                        USING (SELECT ? AS tasa_1, ? AS tasa_2, ? AS tasa_3, ? AS tasa_4, ? AS tasa_5, 
                                      ? AS tasa_6, ? AS tasa_7, ? AS oferta_max, ? AS id_plazo_banco, 
                                      ? AS CAPACIDAD_PAGO_MEN, ? AS id_campana_grupo_banco, 
                                      ? AS id_color_banco, ? AS id_usuario_banco, 
                                      ? AS id_rango_deuda, ? AS num_entidades, ? AS frescura, 
                                      ? AS fecha_subida, ? AS dni) AS source
                        ON target.dni = source.dni
                        WHEN MATCHED THEN 
                            UPDATE SET target.tasa_1 = source.tasa_1,
                                       target.tasa_2 = source.tasa_2,
                                       target.tasa_3 = source.tasa_3,
                                       target.tasa_4 = source.tasa_4,
                                       target.tasa_5 = source.tasa_5,
                                       target.tasa_6 = source.tasa_6,
                                       target.tasa_7 = source.tasa_7,
                                       target.oferta_max = source.oferta_max,
                                       target.id_plazo_banco = source.id_plazo_banco,
                                       target.CAPACIDAD_PAGO_MEN = source.CAPACIDAD_PAGO_MEN,
                                       target.id_campana_grupo_banco = source.id_campana_grupo_banco,
                                       target.id_color_banco = source.id_color_banco,
                                       target.id_usuario_banco = source.id_usuario_banco,
                                       target.id_rango_deuda = source.id_rango_deuda,
                                       target.num_entidades = source.num_entidades,
                                       target.frescura = source.frescura,
                                       target.fecha_subida = source.fecha_subida
                        WHEN NOT MATCHED THEN 
                            INSERT ([dni], [tasa_1], [tasa_2], [tasa_3], [tasa_4], [tasa_5], [tasa_6], [tasa_7], 
                                    [oferta_max], [id_plazo_banco], [CAPACIDAD_PAGO_MEN], [id_campana_grupo_banco], 
                                    [id_color_banco], [id_usuario_banco], [id_rango_deuda], [num_entidades], [frescura], [fecha_subida])
                            VALUES (source.dni, source.tasa_1, source.tasa_2, source.tasa_3, source.tasa_4, 
                                    source.tasa_5, source.tasa_6, source.tasa_7, source.oferta_max, source.id_plazo_banco, 
                                    source.CAPACIDAD_PAGO_MEN, source.id_campana_grupo_banco, source.id_color_banco, 
                                    source.id_usuario_banco, source.id_rango_deuda, source.num_entidades, source.frescura, source.fecha_subida);
                    """, datos_a_procesar)
                    
                    contador += 1
                    print("Actualizando lote de 1000 filas... ESTO DEMORARA UN POCO", contador)
                    datos_a_procesar = []

            # Procesar registros restantes
            if datos_a_procesar:
                print("Actualizando restante... ESTAMOS TERMINANDO")
                cursor.executemany("""
                    MERGE INTO [CORE_ALFIN].[dbo].[base_clientes_banco] AS target
                    USING (SELECT ? AS tasa_1, ? AS tasa_2, ? AS tasa_3, ? AS tasa_4, ? AS tasa_5, 
                                  ? AS tasa_6, ? AS tasa_7, ? AS oferta_max, ? AS id_plazo_banco, 
                                  ? AS CAPACIDAD_PAGO_MEN, ? AS id_campana_grupo_banco, 
                                  ? AS id_color_banco, ? AS id_usuario_banco, 
                                  ? AS id_rango_deuda, ? AS num_entidades, ? AS frescura, 
                                  ? AS fecha_subida, ? AS dni) AS source
                    ON target.dni = source.dni
                    WHEN MATCHED THEN 
                        UPDATE SET target.tasa_1 = source.tasa_1,
                                   target.tasa_2 = source.tasa_2,
                                   target.tasa_3 = source.tasa_3,
                                   target.tasa_4 = source.tasa_4,
                                   target.tasa_5 = source.tasa_5,
                                   target.tasa_6 = source.tasa_6,
                                   target.tasa_7 = source.tasa_7,
                                   target.oferta_max = source.oferta_max,
                                   target.id_plazo_banco = source.id_plazo_banco,
                                   target.CAPACIDAD_PAGO_MEN = source.CAPACIDAD_PAGO_MEN,
                                   target.id_campana_grupo_banco = source.id_campana_grupo_banco,
                                   target.id_color_banco = source.id_color_banco,
                                   target.id_usuario_banco = source.id_usuario_banco,
                                   target.id_rango_deuda = source.id_rango_deuda,
                                   target.num_entidades = source.num_entidades,
                                   target.frescura = source.frescura,
                                   target.fecha_subida = source.fecha_subida;
                """, datos_a_procesar)
            
            conn.commit()
            
            print("Datos actualizados exitosamente.")

    except Exception as e:
        print("Error al actualizar los datos:", e)

    finally:
        cursor.close()
        conn.close()

# Archivo CSV
csv_file = 'HOJAFEBRERO1.csv'

# Ejecutar función
actualizar_datos_csv(csv_file)
