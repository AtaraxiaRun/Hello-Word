pipeline {
    agent any

    stages {
        stage('Build') {
            steps {
                // 在此处进行构建操作，生成镜像
                // 例如使用 Dockerfile 构建项目镜像
                sh 'docker build -t consoleapp6-image:1.0 .'
            }
        }

        stage('Publish') {
            steps {
                script {
                    // 登录到 Harbor
                    withDockerRegistry([credentialsId: 'Harbor_User_002', url: 'http://192.168.226.130:8082']) {
                        // 标记镜像
                        sh 'docker tag myproject:1.0 192.168.226.130:8082/text-cloud/myproject:1.0'

                        // 推送镜像到 Harbor
                        sh 'docker push 192.168.226.130:8082/text-cloud/myproject:1.0'
                    }
                }
            }
        }
    }
}
